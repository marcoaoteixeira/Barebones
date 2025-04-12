using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.VisualBasic;
using Nameless.Barebones.Domains.Entities.Identity;

namespace Nameless.Barebones.Domains.Configurations.Identity;

public sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<User> {
    public void Configure(EntityTypeBuilder<User> builder) {
        builder.ToTable("Users");

        // Key
        builder.Property(entity => entity.Id)
               .ValueGeneratedOnAdd();

        builder.HasKey(entity => entity.Id);

        // Fields
        builder.Property(entity => entity.FirstName)
               .HasMaxLength(256);

        builder.Property(entity => entity.LastName)
               .HasMaxLength(256);

        builder.Property(entity => entity.UserName)
               .HasMaxLength(256);

        builder.Property(entity => entity.NormalizedUserName)
               .HasMaxLength(256);

        builder.Property(entity => entity.Email)
               .HasMaxLength(256);

        builder.Property(entity => entity.NormalizedEmail)
               .HasMaxLength(256);

        builder.Property(entity => entity.EmailConfirmed);

        builder.Property(entity => entity.PhoneNumber)
               .HasMaxLength(64);

        builder.Property(entity => entity.PhoneNumberConfirmed);

        builder.Property(entity => entity.AvatarUrl)
               .HasMaxLength(1024);

        builder.Property(entity => entity.LockoutEnabled);

        builder.Property(entity => entity.LockoutEnd)
               .IsRequired(false);

        builder.Property(entity => entity.PasswordHash)
               .HasMaxLength(4096);

        builder.Property(entity => entity.SecurityStamp)
               .HasMaxLength(4096);

        builder.Property(entity => entity.TwoFactorEnabled);

        builder.Property(entity => entity.AccessFailedCount);

        builder.Property<string>(Constants.Database.Fields.ConcurrencyStampColumnName)
               .IsConcurrencyToken()
               .HasMaxLength(int.MaxValue);

        // Indexes
        builder.HasIndex(entity => entity.NormalizedUserName)
               .IsUnique()
               .HasDatabaseName("UserNameIndex")
               .HasFilter("[NormalizedUserName] IS NOT NULL");

        builder.HasIndex(entity => entity.NormalizedEmail)
               .IsUnique()
               .HasDatabaseName("EmailIndex")
               .HasFilter("[NormalizedEmail] IS NOT NULL");

        // Each User can have many UserClaims
        builder.HasMany(entity => entity.Claims)
               .WithOne(entity => entity.User)
               .HasForeignKey(userClaim => userClaim.UserId)
               .IsRequired();

        // Each User can have many UserLogins
        builder.HasMany(entity => entity.Logins)
               .WithOne(entity => entity.User)
               .HasForeignKey(userLogin => userLogin.UserId)
               .IsRequired();

        // Each User can have many UserTokens
        builder.HasMany(entity => entity.Tokens)
               .WithOne(entity => entity.User)
               .HasForeignKey(userToken => userToken.UserId)
               .IsRequired();

        // Each User can have many entries in the UserRole join table
        builder.HasMany(entity => entity.UserRoles)
               .WithOne(entity => entity.User)
               .HasForeignKey(userRole => userRole.UserId)
               .IsRequired();
    }
}