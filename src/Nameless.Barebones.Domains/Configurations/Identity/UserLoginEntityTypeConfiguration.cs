using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Barebones.Domains.Entities.Identity;

namespace Nameless.Barebones.Domains.Configurations.Identity;

public sealed class UserLoginEntityTypeConfiguration : IEntityTypeConfiguration<UserLogin> {
    public void Configure(EntityTypeBuilder<UserLogin> builder) {
        builder.ToTable("UserLogins");

        builder.HasKey(entity => new { entity.LoginProvider, entity.ProviderKey });

        builder.Property(entity => entity.LoginProvider)
               .HasMaxLength(512);

        builder.Property(entity => entity.ProviderKey)
               .HasMaxLength(512);

        builder.Property(entity => entity.ProviderDisplayName)
               .HasMaxLength(4096);

        builder.Property(entity => entity.UserId);

        builder.HasIndex(entity => entity.UserId);
    }
}