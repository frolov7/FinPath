using FinPath.Application.Common.Interfaces;
using FinPath.Infrastructure.Authentication;

namespace FinPath.UnitTests.Authentication
{
    public sealed class PasswordHasherTests
    {
        private readonly PasswordHasher _passwordHasher = new();

        [Fact]
        public void HashPassword_ShouldReturnHash_WhenPasswordIsValid()
        {
            // Arrange
            const string password = "Password1!";

            // Act
            var passwordHash =
                _passwordHasher.HashPassword(password);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(passwordHash));
            Assert.NotEqual(password, passwordHash);
        }

        [Fact]
        public void HashPassword_ShouldReturnDifferentHashes_WhenPasswordIsSame()
        {
            // Arrange
            const string password = "Password1!";

            // Act
            var firstHash =
                _passwordHasher.HashPassword(password);

            var secondHash =
                _passwordHasher.HashPassword(password);

            // Assert
            Assert.NotEqual(firstHash, secondHash);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsCorrect()
        {
            // Arrange
            const string password = "Password1!";

            var passwordHash =
                _passwordHasher.HashPassword(password);

            // Act
            var result = _passwordHasher.VerifyPassword(
                passwordHash,
                password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsIncorrect()
        {
            // Arrange
            const string correctPassword = "Password1!";
            const string incorrectPassword = "DifferentPassword1!";

            var passwordHash =
                _passwordHasher.HashPassword(correctPassword);

            // Act
            var result = _passwordHasher.VerifyPassword(
                passwordHash,
                incorrectPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HashPassword_ShouldThrowArgumentNullException_WhenPasswordIsNull()
        {
            // Arrange
            string password = null!;

            // Act
            var exception = Assert.Throws<ArgumentNullException>(
                () => _passwordHasher.HashPassword(password));

            // Assert
            Assert.Equal("password", exception.ParamName);
        }

        [Fact]
        public void VerifyPassword_ShouldThrowArgumentNullException_WhenPasswordHashIsNull()
        {
            // Arrange
            string passwordHash = null!;
            const string password = "Password1!";

            // Act
            var exception = Assert.Throws<ArgumentNullException>(
                () => _passwordHasher.VerifyPassword(
                    passwordHash,
                    password));

            // Assert
            Assert.Equal("passwordHash", exception.ParamName);
        }

        [Fact]
        public void VerifyPassword_ShouldThrowArgumentNullException_WhenPasswordIsNull()
        {
            // Arrange
            const string passwordHash = "some-password-hash";
            string password = null!;

            // Act
            var exception = Assert.Throws<ArgumentNullException>(
                () => _passwordHasher.VerifyPassword(
                    passwordHash,
                    password));

            // Assert
            Assert.Equal("password", exception.ParamName);
        }
    }
}