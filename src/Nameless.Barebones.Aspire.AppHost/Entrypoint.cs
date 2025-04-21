using Nameless.Barebones.Aspire.AppHost.Configs;

namespace Nameless.Barebones.Aspire.AppHost;

public static class Entrypoint {
    public static void Main(string[] args)
        => DistributedApplication.CreateBuilder(args)
                                 .RegisterFrontendApplication()
                                 .Build()
                                 .Run();
}