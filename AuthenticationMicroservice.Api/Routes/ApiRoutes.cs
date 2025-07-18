namespace AuthenticationMicroservice.Api.Routes;

public static class ApiRoutes
{
    public static class AuthenticationController
    {
        public const string Base = "Authentication";

        public const string Login = "Login";
        public const string Register = "Register";
        public const string Logout = "Logout";
        public const string RefreshAccessToken = "RefreshAccessToken";
        public const string GetNewRefreshToken = "GetNewRefreshToken";
    }
}
