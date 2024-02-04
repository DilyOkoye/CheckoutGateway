using IdentityServer4.Models;
using IdentityServer4.Test;

namespace PaymentGateway.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
       

        public static IEnumerable<ApiScope> GetApiScopes() =>
            new List<ApiScope>
            {
                new ApiScope("api1", "My API"),
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "Apple",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("678ebc03-8fb1-407f-ac5e-ff97e8b810f5".Sha256())
                    },
                    AllowedScopes = { "PaymentGatewayApi" }
                },
            };

        // Define test users (optional for development purposes)
        public static List<TestUser> GetTestUsers() =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "admin",
                    Password = "admin",
                },
            };
    }

}
