using FinPath.Application.Common.Exceptions;
using FinPath.Application.Common.Interfaces;
using FinPath.Application.Users.Register;
using FinPath.Domain.Users;
using Moq;
using System.Timers;

namespace FinPath.UnitTests.Users.Register
{
    public sealed class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new RegisterUserCommandHandler(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateUser_WhenEmailIsNotRegistered()
        {
            // Arrange
            var command = new RegisterUserCommand(
                Email: "user@example.com",
                DisplayName: "Иван",
                Password: "Password1!");

            const string passwordHash = "hashed-password";

            _userRepositoryMock
                .Setup(repository =>
                    repository.ExistsByNormalizedEmailAsync(
                        "USER@EXAMPLE.COM",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.HashPassword(command.Password))
                .Returns(passwordHash);

            _userRepositoryMock
                .Setup(repository =>
                    repository.AddAsync(
                        It.IsAny<User>(),
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(command.Email, result.Email);
            Assert.Equal(command.DisplayName, result.DisplayName);

            _passwordHasherMock.Verify(
                hasher => hasher.HashPassword(command.Password),
                Times.Once);

            _userRepositoryMock.Verify(
                repository => repository.AddAsync(
                    It.Is<User>(user =>
                        user.Id == result.Id &&
                        user.Email == command.Email &&
                        user.NormalizedEmail == "USER@EXAMPLE.COM" &&
                        user.DisplayName == command.DisplayName &&
                        user.PasswordHash == passwordHash),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork => unitOfWork.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNormalizeEmailBeforeCheckingForDuplicate()
        {
            // Arrange
            var command = new RegisterUserCommand(
                Email: "  User@Example.com  ",
                DisplayName: "Иван",
                Password: "Password1!");

            const string passwordHash = "hashed-password";

            _userRepositoryMock
                .Setup(repository =>
                    repository.ExistsByNormalizedEmailAsync(
                        "USER@EXAMPLE.COM",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.HashPassword(command.Password))
                .Returns(passwordHash);

            _userRepositoryMock
                .Setup(repository =>
                    repository.AddAsync(
                        It.IsAny<User>(),
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.Equal("User@Example.com", result.Email);

            _userRepositoryMock.Verify(
                repository =>
                    repository.ExistsByNormalizedEmailAsync(
                        "USER@EXAMPLE.COM",
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowConflictException_WhenEmailIsAlreadyRegistered()
        {
            // Arrange
            var command = new RegisterUserCommand(
                Email: "user@example.com",
                DisplayName: "Иван",
                Password: "Password1!");

            var existingUser = User.Create(
                email: "user@example.com",
                displayName: "Существующий пользователь",
                passwordHash: "existing-password-hash");

            _userRepositoryMock
                .Setup(repository =>
                    repository.ExistsByNormalizedEmailAsync(
                        "USER@EXAMPLE.COM",
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var exception = await Assert.ThrowsAsync<ConflictException>(
                () => _handler.Handle(
                    command,
                    CancellationToken.None));

            // Assert
            Assert.Equal(
                "Пользователь с таким адресом электронной почты " +
                "уже зарегистрирован.",
                exception.Message);

            _passwordHasherMock.Verify(
                hasher => hasher.HashPassword(
                    It.IsAny<string>()),
                Times.Never);

            _userRepositoryMock.Verify(
                repository => repository.AddAsync(
                    It.IsAny<User>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                unitOfWork => unitOfWork.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToRepositoryAndUnitOfWork()
        {
            // Arrange
            var command = new RegisterUserCommand(
                Email: "user@example.com",
                DisplayName: "Иван",
                Password: "Password1!");

            using var cancellationTokenSource =
                new CancellationTokenSource();

            var cancellationToken =
                cancellationTokenSource.Token;

            _userRepositoryMock
                .Setup(repository =>
                    repository.ExistsByNormalizedEmailAsync(
                        "USER@EXAMPLE.COM",
                        cancellationToken))
                .ReturnsAsync(false);

            _passwordHasherMock
                .Setup(hasher =>
                    hasher.HashPassword(command.Password))
                .Returns("hashed-password");

            _userRepositoryMock
                .Setup(repository =>
                    repository.AddAsync(
                        It.IsAny<User>(),
                        cancellationToken))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(unitOfWork =>
                    unitOfWork.SaveChangesAsync(
                        cancellationToken))
                .ReturnsAsync(1);

            // Act
            await _handler.Handle(
                command,
                cancellationToken);

            // Assert
            _userRepositoryMock.Verify(
                repository =>
                    repository.ExistsByNormalizedEmailAsync(
                        "USER@EXAMPLE.COM",
                        cancellationToken),
                Times.Once);

            _userRepositoryMock.Verify(
                repository => repository.AddAsync(
                    It.IsAny<User>(),
                    cancellationToken),
                Times.Once);

            _unitOfWorkMock.Verify(
                unitOfWork =>
                    unitOfWork.SaveChangesAsync(cancellationToken),
                Times.Once);
        }
    }
}