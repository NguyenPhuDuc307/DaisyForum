using IdentityServer4;
using IdentityServer4.Models;

namespace DaisyForum.BackendServer.IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

        public static IEnumerable<ApiResource> Apis =>
        new ApiResource[]
        {
            new ApiResource("api.daisyforum", "DaisyForum API")
        };

        public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
                new ApiScope("api.daisyforum", "DaisyForum API")
        };

        
    }
}