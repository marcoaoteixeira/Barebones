namespace Nameless.WebApplication.Aspire.AppHost;

public static class EntryPoint {
    public static void Main(string[] args) {
        var builder = DistributedApplication.CreateBuilder(args);

        builder.Build().Run();
    }
}