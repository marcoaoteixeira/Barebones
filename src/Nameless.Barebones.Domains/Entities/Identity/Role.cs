using Microsoft.AspNetCore.Identity;

namespace Nameless.Barebones.Domains.Entities.Identity;

public class Role : IdentityRole<Guid> {
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
    public virtual ICollection<RoleClaim> RoleClaims { get; set; } = [];
}