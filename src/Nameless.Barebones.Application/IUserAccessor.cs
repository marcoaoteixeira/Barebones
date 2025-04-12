using Nameless.WebApplication.Domains.Entities.Identity;

namespace Nameless.WebApplication.Application;

public interface IUserAccessor {
    Task<User> GetCurrentUserAsync();
}