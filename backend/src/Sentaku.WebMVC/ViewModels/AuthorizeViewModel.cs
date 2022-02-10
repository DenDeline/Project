namespace Sentaku.WebMVC.ViewModels
{
  public class AuthorizeViewModel
  {
    public string Login { get; set; }
    public string Password { get; set; }
    public string State { get; set; }
    public string ClientId { get; set; }
    public string RedirectUri { get; set; }
    public string CodeChallenge { get; set; }
    public string CodeChallengeMethod { get; set; }
  }
}
