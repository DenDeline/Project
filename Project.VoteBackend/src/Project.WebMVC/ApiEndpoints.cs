namespace Project.WebMVC
{
    public static class ApiEndpoints
    {
        public const string BasePath = "https://localhost:44307/api";
        
        public static class Users
        {
            public static string GetUserRoles(string username) => $"{BasePath}/users/{username}/roles";
        }
    }
}