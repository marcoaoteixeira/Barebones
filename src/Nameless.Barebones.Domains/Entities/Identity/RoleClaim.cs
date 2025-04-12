using Microsoft.AspNetCore.Identity;

namespace Nameless.Barebones.Domains.Entities.Identity;

public class RoleClaim : IdentityRoleClaim<Guid> {
    public virtual Role? Role { get; set; }
}