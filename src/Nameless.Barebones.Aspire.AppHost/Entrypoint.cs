using Projects;

namespace Nameless.Barebones.Aspire.AppHost;

public static class Entrypoint {
    public static void Main(string[] args)
        => DistributedApplication.CreateBuilder(args)
                                 .AddProject<Nameless_Barebones_Web>("app")
                                 .ApplicationBuilder
                                 .Build()
                                 .Run();
}