namespace Dddify.Localization;

public class JsonLocalizationOptions
{
    public JsonLocalizationOptions(string resourcesPath, string[] supportedCultures, string defaultCulture)
    {
        ResourcesPath = resourcesPath;
        SupportedCultures = supportedCultures;
        DefaultCulture = defaultCulture;
    }

    public JsonLocalizationOptions()
    {
    }

    public string ResourcesPath { get; set; } = "Resources";

    public string[] SupportedCultures { get; set; } = Array.Empty<string>();

    public string? DefaultCulture { get; set; }
}