using System.Net;
using FastEndpoints;

namespace Nameless.Barebones.Web.Endpoints.Groups;

public sealed class AccountsGroup : Group {
    public AccountsGroup() {
        Configure("accounts", definition => {
            definition.Description(builder => {
                builder.Produces(statusCode: (int)HttpStatusCode.OK)
                       .Produces(statusCode: (int)HttpStatusCode.Unauthorized)
                       .Produces(statusCode: (int)HttpStatusCode.Forbidden);
            });

            definition.AllowAnonymous();
            definition.EndpointVersion(version: 1);
        });
    }
}