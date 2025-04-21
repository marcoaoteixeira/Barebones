namespace Nameless.Barebones.Web.Configs;

internal static class LocalizationConfig {
    internal static IServiceCollection RegisterLocalizationServices(this IServiceCollection self)
        => self.AddLocalization(options => options.ResourcesPath = "Resources");
}
