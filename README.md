Dddify
=======

[![NuGet](https://img.shields.io/nuget/vpre/dddify.svg)](https://www.nuget.org/packages/dddify)

A lightweight framework that practices Domain-Driven Design approach for building modern applications on ASP.NET Core.

### Install

You should install [Dddify with NuGet](https://www.nuget.org/packages/dddify):

    Install-Package Dddify
    
Or via the .NET Core command line interface:

    dotnet add package Dddify

Either commands, from Package Manager Console or .NET Core CLI, will download and install Dddify and all required dependencies. Currently only supports the target framework .NET 7.


### Usage

Add the following code to the Program.cs file.

    builder.Services.AddDddify(cfg =>
    {
        // Sets the DateTimeKind for date and time values.
        cfg.WithDateTimeKind(DateTimeKind.Utc);

        // Sets the type of sequential GUID to be used.
        cfg.WithSequentialGuidType(SequentialGuidType.SequentialAsString);

        // Adds the JSON localization extension
        cfg.UseJsonLocalization();

        // Adds the API result wrapper extension.
        cfg.UseApiResultWrapper();
    });

