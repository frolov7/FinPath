namespace FinPath.Api.Contracts.Authentication
{
    public sealed class RegisterUserRequest
    {
        public string Email { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
