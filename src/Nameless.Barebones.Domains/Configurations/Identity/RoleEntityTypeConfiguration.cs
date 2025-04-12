using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.VisualBasic;
using Nameless.Barebones.Domains.Entities.Identity;

namespace Nameless.Barebones.Domains.Configurations.Identity;

public sealed class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role> {
    public void Configure(EntityTypeBuilder<Role> builder) {
        builder.ToTable("Roles");

        builder.Property(entity => entity.Id)
               .ValueGeneratedOnAdd();

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Name)
               .HasMaxLength(256);

        builder.Property(entity => entity.NormalizedName)
               .HasMaxLength(256);

        builder.Property<string>(Constants.Database.Fields.ConcurrencyStampColumnName)
               .HasMaxLength(int.MaxValue)
               .IsConcurrencyToken();

        builder.HasIndex(entity => entity.NormalizedName)
               .IsUnique()
               .HasDatabaseName("RoleNameIndex")
               .HasFilter("[NormalizedName] IS NOT NULL");

        // Each Role can have many entries in the UserRole join table
        builder.HasMany(entity => entity.UserRoles)
               .WithOne(entity => entity.Role)
               .HasForeignKey(userRole => userRole.RoleId)
               .IsRequired();

        // Each Role can have many associated RoleClaims
        builder.HasMany(entity => entity.RoleClaims)
               .WithOne(entity => entity.Role)
               .HasForeignKey(roleClaim => roleClaim.RoleId)
               .IsRequired();
    }
}