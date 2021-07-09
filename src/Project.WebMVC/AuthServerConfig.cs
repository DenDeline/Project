using System.Collections.Generic;
using Project.WebMVC.Models.AuthServer;

namespace Project.WebMVC
{
    public static class AuthServerConfig
    {
        public static IReadOnlyList<string> SupportedResponseTypes =>
            new List<string>
            {
                AuthServerConstants.ResponseTypes.Code
            }.AsReadOnly();

        public static IReadOnlyList<string> SupportedGrantTypes =>
            new List<string>
            {
                AuthServerConstants.GrantTypes.AuthorizationCode
            }.AsReadOnly();
        
        //TODO: SHA256-HMAC alg for client secret
        public static IReadOnlyList<Client> InMemoryClients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "project_next-js_8f62ee4312924427b386026f83028dff",
                    ClientSecret = "b14ca5898a4e4133bbce2ea2315a1916",
                    RedirectUris = { "http://localhost:3000/oauth2callback" }
                }
            }.AsReadOnly();
    }
}