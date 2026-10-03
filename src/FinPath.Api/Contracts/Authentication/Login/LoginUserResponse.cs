namespace FinPath.Api.Contracts.Authentication.Login
{
    public sealed class LoginUserResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public DateTimeOffset ExpiresAtUtc { get; init; }
    }
}