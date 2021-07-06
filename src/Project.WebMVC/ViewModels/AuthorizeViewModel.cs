namespace Project.WebMVC.ViewModels
{
    public class AuthorizeViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string State { get; set; }
        public string RedirectUri { get; set; }
    }
}