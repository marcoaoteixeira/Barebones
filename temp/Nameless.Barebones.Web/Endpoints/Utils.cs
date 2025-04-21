namespace Nameless.Barebones.Web.Endpoints;

internal static class Utils {
    internal static string RemoveRedundantProvider(string provider) {
        // Temporary workaround for FluentButton returning a provider value twice
        // Split the comma-separated list of strings
        var providers = provider.Split(',');

        // Find the value that appears twice in the list
        provider = providers.GroupBy(value => value)
                            .Where(group => group.Count() == 2)
                            .Select(group => group.Key)
                            .First();
        return provider;
    }
}
