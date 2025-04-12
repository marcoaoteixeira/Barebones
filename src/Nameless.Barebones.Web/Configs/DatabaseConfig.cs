using Microsoft.EntityFrameworkCore;
using Nameless.Barebones.Domains;

namespace Nameless.Barebones.Web.Configs;

internal static class DatabaseConfig {
    internal static IServiceCollection RegisterDatabaseServices(this IServiceCollection self, IConfiguration configuration) {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (connectionString is null) {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        self.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
        self.AddDatabaseDeveloperPageExceptionFilter();

        return self;
    }
}
