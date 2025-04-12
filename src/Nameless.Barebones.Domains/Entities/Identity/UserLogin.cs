using Microsoft.AspNetCore.Identity;

namespace Nameless.Barebones.Domains.Entities.Identity;

public class UserLogin : IdentityUserLogin<Guid> {
    public virtual User? User { get; set; }
}