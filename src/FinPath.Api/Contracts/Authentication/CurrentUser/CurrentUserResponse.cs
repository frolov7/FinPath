namespace FinPath.Api.Contracts.Authentication.CurrentUser
{
    public sealed class CurrentUserResponse
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
    }
}
