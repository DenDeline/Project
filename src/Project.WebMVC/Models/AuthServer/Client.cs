using System.Collections.Generic;

namespace Project.WebMVC.Models.AuthServer
{
    public class Client
    {
        public string ClientId { get; set; }
        public ICollection<string> RedirectUris { get; set; } = new HashSet<string>();
        public string ClientSecret { get; set; }
    }
}