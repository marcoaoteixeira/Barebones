using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Nameless.Barebones.Domains.Entities.Identity;

public class User : IdentityUser<Guid> {
    [MaxLength(256)]
    public string? FirstName { get; set; }

    [MaxLength(256)]
    public string? LastName { get; set; }

    [MaxLength(1024)]
    public string? AvatarUrl { get; set; }

    public virtual ICollection<UserClaim> Claims { get; set; } = [];

    public virtual ICollection<UserLogin> Logins { get; set; } = [];

    public virtual ICollection<UserToken> Tokens { get; set; } = [];

    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
}