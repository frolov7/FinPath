namespace FinPath.Api.Contracts.Authentication.Register
{
    public sealed class RegisterUserResponse
    {
        public Guid Id { get; init; }

        public string Email { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;
    }
}