using Microsoft.AspNetCore.Identity;

namespace Nameless.Barebones.Domains.Entities.Identity;

public class UserToken : IdentityUserToken<Guid> {
    public virtual User? User { get; set; }
}