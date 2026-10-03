using FinPath.Application.Users.Login;
using FinPath.Domain.Users;
using FluentValidation.TestHelper;

namespace FinPath.UnitTests.Users.Login
{
    public sealed class LoginUserCommandValidatorTests
    {
        private readonly LoginUserCommandValidator _validator = new();

        [Fact]
        public void Validate_ShouldNotHaveErrors_WhenCommandIsValid()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailIsEmpty()
        {
            // Arrange
            var command = CreateValidCommand() with
            {
                Email = string.Empty
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(
                    currentCommand => currentCommand.Email)
                .WithErrorMessage(
                    "Адрес электронной почты обязателен.");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailContainsOnlyWhitespace()
        {
            // Arrange
            var command = CreateValidCommand() with
            {
                Email = "   "
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(
                    currentCommand => currentCommand.Email)
                .WithErrorMessage(
                    "Адрес электронной почты обязателен.");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailHasInvalidFormat()
        {
            // Arrange
            var command = CreateValidCommand() with
            {
                Email = "invalid-email"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(
                    currentCommand => currentCommand.Email)
                .WithErrorMessage(
                    "Адрес электронной почты имеет некорректный формат.");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailExceedsMaximumLength()
        {
            // Arrange
            const string domain = "@example.com";

            var command = CreateValidCommand() with
            {
                Email = new string(
                    'a',
                    User.MaxEmailLength - domain.Length + 1) + domain
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(
                    currentCommand => currentCommand.Email)
                .WithErrorMessage(
                    $"Адрес электронной почты не должен превышать " +
                    $"{User.MaxEmailLength} символов.");
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenEmailLengthEqualsMaximum()
        {
            // Arrange
            const string domain = "@example.com";

            var command = CreateValidCommand() with
            {
                Email = new string(
                    'a',
                    User.MaxEmailLength - domain.Length) + domain
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(
                currentCommand => currentCommand.Email);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPasswordIsEmpty()
        {
            // Arrange
            var command = CreateValidCommand() with
            {
                Password = string.Empty
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(
                    currentCommand => currentCommand.Password)
                .WithErrorMessage("Пароль обязателен.");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPasswordContainsOnlyWhitespace()
        {
            // Arrange
            var command = CreateValidCommand() with
            {
                Password = "   "
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(
                    currentCommand => currentCommand.Password)
                .WithErrorMessage("Пароль обязателен.");
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenPasswordContainsInnerWhitespace()
        {
            // Arrange
            var command = CreateValidCommand() with
            {
                Password = "Password 1!"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(
                currentCommand => currentCommand.Password);
        }

        private static LoginUserCommand CreateValidCommand()
        {
            return new LoginUserCommand(
                Email: "user@example.com",
                Password: "Password1!");
        }
    }
}