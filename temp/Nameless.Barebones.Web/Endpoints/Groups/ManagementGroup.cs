using System.Net;
using FastEndpoints;

namespace Nameless.Barebones.Web.Endpoints.Groups;

public class ManagementGroup : SubGroup<AccountsGroup> {
    public ManagementGroup() {
        Configure("management", definition => {
            definition.Description(builder => {
                builder.Produces(statusCode: (int)HttpStatusCode.OK)
                       .Produces(statusCode: (int)HttpStatusCode.Unauthorized)
                       .Produces(statusCode: (int)HttpStatusCode.Forbidden);
            });

            definition.EndpointVersion(version: 1);
        });
    }
}
