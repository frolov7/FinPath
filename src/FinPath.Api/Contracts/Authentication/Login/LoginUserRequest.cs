namespace FinPath.Api.Contracts.Authentication.Login
{
    public sealed class LoginUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
