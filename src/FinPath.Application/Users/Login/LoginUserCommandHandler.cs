using FinPath.Application.Common.Exceptions;
using FinPath.Application.Common.Interfaces;
using MediatR;

namespace FinPath.Application.Users.Login
{
    public sealed class LoginUserCommandHandler
        : IRequestHandler<LoginUserCommand, LoginUserResult>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public LoginUserCommandHandler(
            IJwtTokenGenerator jwtTokenGenerator,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginUserResult> Handle(
            LoginUserCommand command,
            CancellationToken cancellationToken)
        {
            var normalizedEmail = command.Email
                .Trim()
                .ToUpperInvariant();

            var user =
                await _userRepository.GetByNormalizedEmailAsync(
                    normalizedEmail,
                    cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException(
                    "Неверный адрес электронной почты или пароль.");
            }

            var isPasswordValid = _passwordHasher.VerifyPassword(
                user.PasswordHash,
                command.Password);

            if (!isPasswordValid)
            {
                throw new UnauthorizedException(
                    "Неверный адрес электронной почты или пароль.");
            }

            var accessToken = _jwtTokenGenerator.Generate(user);

            return new LoginUserResult(
                accessToken.Value,
                accessToken.ExpiresAtUtc);
        }
    }
}