using System.Collections.Generic;
using Project.WebMVC.Controllers;

namespace Project.WebMVC
{
    public static class AuthServerConfig
    {
        public static IReadOnlyList<string> SupportedResponseTypes =>
            new List<string>
            {
                AuthServerConstants.ResponseTypes.Code
            }.AsReadOnly();
        
        public static IReadOnlyList<Client> InMemoryClients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "project_next-js_8f62ee4312924427b386026f83028dff",
                    ClientSecrets = { "very-secret-value" },
                    RedirectUris = { "http://localhost:3000/oauth2callback" }
                }
            }.AsReadOnly();
    }
}