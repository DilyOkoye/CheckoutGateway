using System.Security.Claims;
using IdentityServer4.Test;

namespace PaymentGateway.IdentityServer
{
    public static class TestUsers
    {
        public static List<TestUser> Users => new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "alice",
                Password = "password",
                Claims = new List<Claim>
                {
                    new Claim("name", "Alice"),
                    new Claim("website", "https://alice.com")
                }
            },
            new TestUser
            {
                SubjectId = "2",
                Username = "bob",
                Password = "password",
                Claims = new List<Claim>
                {
                    new Claim("name", "Bob"),
                    new Claim("website", "https://bob.com")
                }
            }
        };
    }

}
