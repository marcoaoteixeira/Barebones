using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nameless.Barebones.Domains.Configurations.Identity;
using Nameless.Barebones.Domains.Entities.Identity;

namespace Nameless.Barebones.Domains;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken> {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.ApplyConfiguration(new RoleClaimEntityTypeConfiguration());
        builder.ApplyConfiguration(new RoleEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserClaimEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserLoginEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
        builder.ApplyConfiguration(new UserTokenEntityTypeConfiguration());
    }
}