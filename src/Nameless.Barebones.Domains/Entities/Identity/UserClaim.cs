using Microsoft.AspNetCore.Identity;

namespace Nameless.Barebones.Domains.Entities.Identity;

public class UserClaim : IdentityUserClaim<Guid> {
    public virtual User? User { get; set; }
}