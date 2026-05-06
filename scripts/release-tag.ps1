param(
    [switch]$Push,
    [string]$Remote = "origin",
    [switch]$Force,
    [switch]$Delete,
    [switch]$DeleteRemote
)

$ErrorActionPreference = "Stop"

function Get-PackageVersion {
    param(
        [Parameter(Mandatory = $true)]
        [string]$PropsPath
    )

    if (-not (Test-Path $PropsPath)) {
        throw "Could not find '$PropsPath'."
    }

    [xml]$props = Get-Content $PropsPath
    $group = $props.Project.PropertyGroup

    $prefix = $group.VersionPrefix
    $suffix = $group.VersionSuffix

    if ([string]::IsNullOrWhiteSpace($prefix)) {
        throw "VersionPrefix is missing in '$PropsPath'."
    }

    if ([string]::IsNullOrWhiteSpace($suffix)) {
        return $prefix
    }

    return "$prefix-$suffix"
}

function Assert-GitRepository {
    git rev-parse --is-inside-work-tree *> $null
    if ($LASTEXITCODE -ne 0) {
        throw "Current directory is not inside a git repository."
    }
}

function Assert-CommitExists {
    git rev-parse HEAD *> $null
    if ($LASTEXITCODE -ne 0) {
        throw "Could not resolve HEAD."
    }
}

Assert-GitRepository
Assert-CommitExists

$repoRoot = git rev-parse --show-toplevel
Set-Location $repoRoot

$propsPath = Join-Path $repoRoot "Directory.Build.props"
$packageVersion = Get-PackageVersion -PropsPath $propsPath
$tagName = "v$packageVersion"

Write-Host "Resolved package version: $packageVersion"
Write-Host "Resolved tag name: $tagName"

$isDeleteMode = $Delete -or $DeleteRemote

if ($isDeleteMode) {
    if ($Push) {
        throw "Do not combine -Push with delete mode. Use -Delete and/or -DeleteRemote."
    }

    $existingLocalTag = git tag --list $tagName
    if ($Delete) {
        if ($existingLocalTag) {
            git tag -d $tagName | Out-Null
            Write-Host "Deleted local tag '$tagName'."
        }
        else {
            Write-Host "Local tag '$tagName' does not exist. Skipping."
        }
    }

    if ($DeleteRemote) {
        git fetch $Remote --tags --force

        $existingRemoteTag = git ls-remote --tags $Remote "refs/tags/$tagName"
        if ($existingRemoteTag) {
            git push $Remote ":refs/tags/$tagName"
            Write-Host "Deleted remote tag '$tagName' from '$Remote'."
        }
        else {
            Write-Host "Remote tag '$tagName' does not exist on '$Remote'. Skipping."
        }
    }

    exit 0
}

$existingLocalTag = git tag --list $tagName
if ($existingLocalTag) {
    if (-not $Force) {
        throw "Tag '$tagName' already exists locally. Use -Force to recreate it."
    }

    git tag -d $tagName | Out-Null
    Write-Host "Deleted existing local tag '$tagName'."
}

if ($Push) {
    git fetch $Remote --tags --force

    $existingRemoteTag = git ls-remote --tags $Remote "refs/tags/$tagName"
    if ($existingRemoteTag) {
        if (-not $Force) {
            throw "Tag '$tagName' already exists on remote '$Remote'. Use -Force only if you intentionally want to replace it."
        }

        git push $Remote ":refs/tags/$tagName"
        Write-Host "Deleted existing remote tag '$tagName' from '$Remote'."
    }
}

git tag $tagName
Write-Host "Created local tag '$tagName'."

if ($Push) {
    git push $Remote $tagName
    Write-Host "Pushed tag '$tagName' to '$Remote'."
}
else {
    Write-Host "Tag was created locally only. Use -Push to publish it."
}
