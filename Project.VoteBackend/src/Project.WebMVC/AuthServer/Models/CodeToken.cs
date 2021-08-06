using System;

namespace Project.WebMVC.AuthServer.Models
{
    public class CodeToken
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string ClientId { get; set; }
        public string UserId { get; set; }
        public DateTime ExpiresIn { get; } = DateTime.UtcNow.AddSeconds(60);
        public string RedirectUri { get; set; }
        public string? CodeChallenge { get; set; }
        public string? CodeChallengeMethod { get; set; }
    }
}
