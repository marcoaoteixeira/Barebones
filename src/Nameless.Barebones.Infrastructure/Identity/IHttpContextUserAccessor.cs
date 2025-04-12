using Microsoft.AspNetCore.Http;
using Nameless.Barebones.Domains.Entities.Identity;

namespace Nameless.Barebones.Infrastructure.Identity;

public interface IHttpContextUserAccessor {
    Task<User> GetCurrentUserAsync(HttpContext httpContext);
}