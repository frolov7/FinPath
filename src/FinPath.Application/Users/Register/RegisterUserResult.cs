namespace FinPath.Application.Users.Register
{
    /// <summary>
    /// Содержит результат регистрации пользователя.
    /// </summary>
    public sealed record RegisterUserResult(Guid Id, string Email, string DisplayName);
}
