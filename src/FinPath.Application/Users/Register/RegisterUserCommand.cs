using MediatR;

namespace FinPath.Application.Users.Register
{
    /// <summary>
    /// Представляет команду регистрации пользователя.
    /// </summary>
    public sealed record RegisterUserCommand(string Email, string DisplayName, string Password) : IRequest<RegisterUserResult>;
}
