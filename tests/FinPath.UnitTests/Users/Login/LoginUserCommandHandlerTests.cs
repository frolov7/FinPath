using FinPath.Application.Common.Authentication;
using FinPath.Application.Common.Exceptions;
using FinPath.Application.Common.Interfaces;
using FinPath.Application.Users.Login;
using FinPath.Domain.Users;
using NSubstitute;

namespace FinPath.UnitTests.Users.Login
{
    public sealed class LoginUserCommandHandlerTests
    {
        private const string PasswordHash = "stored-password-hash";
        private const string Password = "Password1!";

        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly LoginUserCommandHandler _handler;

        public LoginUserCommandHandlerTests()
        {
            _userRepository =
                Substitute.For<IUserRepository>();

            _passwordHasher =
                Substitute.For<IPasswordHasher>();

            _jwtTokenGenerator =
                Substitute.For<IJwtTokenGenerator>();

            _handler = new LoginUserCommandHandler(
                _jwtTokenGenerator,
                _userRepository,
                _passwordHasher);
        }

        [Fact]
        public async Task Handle_ShouldReturnAccessToken_WhenCredentialsAreValid()
        {
            // Arrange
            using var cancellationTokenSource =
                new CancellationTokenSource();

            var cancellationToken =
                cancellationTokenSource.Token;

            var user = CreateUser();

            var command = new LoginUserCommand(
                Email: "  USER@example.com  ",
                Password: Password);

            var expiresAtUtc =
                DateTimeOffset.UtcNow.AddMinutes(30);

            var accessToken = new AccessToken(
                "generated-access-token",
                expiresAtUtc);

            _userRepository
                .GetByNormalizedEmailAsync(
                    user.NormalizedEmail,
                    cancellationToken)
                .Returns(Task.FromResult<User?>(user));

            _passwordHasher
                .VerifyPassword(
                    user.PasswordHash,
                    command.Password)
                .Returns(true);

            _jwtTokenGenerator
                .Generate(user)
                .Returns(accessToken);

            // Act
            var result = await _handler.Handle(
                command,
                cancellationToken);

            // Assert
            Assert.Equal(accessToken.Value, result.AccessToken);
            Assert.Equal(
                accessToken.ExpiresAtUtc,
                result.ExpiresAtUtc);

            await _userRepository
                .Received(1)
                .GetByNormalizedEmailAsync(
                    "USER@EXAMPLE.COM",
                    cancellationToken);

            _passwordHasher
                .Received(1)
                .VerifyPassword(
                    user.PasswordHash,
                    command.Password);

            _jwtTokenGenerator
                .Received(1)
                .Generate(user);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedException_WhenUserDoesNotExist()
        {
            // Arrange
            var command = new LoginUserCommand(
                Email: "missing@example.com",
                Password: Password);

            _userRepository
                .GetByNormalizedEmailAsync(
                    "MISSING@EXAMPLE.COM",
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<User?>(null));

            // Act
            var exception =
                await Assert.ThrowsAsync<UnauthorizedException>(
                    () => _handler.Handle(
                        command,
                        CancellationToken.None));

            // Assert
            Assert.Equal(
                "Неверный адрес электронной почты или пароль.",
                exception.Message);

            _passwordHasher
                .DidNotReceiveWithAnyArgs()
                .VerifyPassword(default!, default!);

            _jwtTokenGenerator
                .DidNotReceiveWithAnyArgs()
                .Generate(default!);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedException_WhenPasswordIsInvalid()
        {
            // Arrange
            var user = CreateUser();

            var command = new LoginUserCommand(
                Email: user.Email,
                Password: "IncorrectPassword1!");

            _userRepository
                .GetByNormalizedEmailAsync(
                    user.NormalizedEmail,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<User?>(user));

            _passwordHasher
                .VerifyPassword(
                    user.PasswordHash,
                    command.Password)
                .Returns(false);

            // Act
            var exception =
                await Assert.ThrowsAsync<UnauthorizedException>(
                    () => _handler.Handle(
                        command,
                        CancellationToken.None));

            // Assert
            Assert.Equal(
                "Неверный адрес электронной почты или пароль.",
                exception.Message);

            _passwordHasher
                .Received(1)
                .VerifyPassword(
                    user.PasswordHash,
                    command.Password);

            _jwtTokenGenerator
                .DidNotReceiveWithAnyArgs()
                .Generate(default!);
        }

        [Fact]
        public async Task Handle_ShouldPassPasswordWithoutModification_WhenPasswordContainsWhitespace()
        {
            // Arrange
            const string passwordWithWhitespace =
                "Password 1!";

            var user = CreateUser();

            var command = new LoginUserCommand(
                Email: user.Email,
                Password: passwordWithWhitespace);

            var accessToken = new AccessToken(
                "generated-access-token",
                DateTimeOffset.UtcNow.AddMinutes(30));

            _userRepository
                .GetByNormalizedEmailAsync(
                    user.NormalizedEmail,
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<User?>(user));

            _passwordHasher
                .VerifyPassword(
                    user.PasswordHash,
                    passwordWithWhitespace)
                .Returns(true);

            _jwtTokenGenerator
                .Generate(user)
                .Returns(accessToken);

            // Act
            await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            _passwordHasher
                .Received(1)
                .VerifyPassword(
                    user.PasswordHash,
                    passwordWithWhitespace);
        }

        private static User CreateUser()
        {
            return User.Create(
                email: "user@example.com",
                displayName: "Иван",
                passwordHash: PasswordHash);
        }
    }
}