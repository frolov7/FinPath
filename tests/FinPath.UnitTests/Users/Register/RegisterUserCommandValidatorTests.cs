using FinPath.Application.Users.Register;
using FinPath.Domain.Users;
using FluentValidation.TestHelper;

namespace FinPath.UnitTests.Users.Register;

public sealed class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

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

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenEmailIsEmpty(string email)
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            Email = email
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Email)
            .WithErrorMessage("Адрес электронной почты обязателен.");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("user")]
    [InlineData("user@")]
    [InlineData("@example.com")]
    public void Validate_ShouldHaveError_WhenEmailFormatIsInvalid(string email)
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            Email = email
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Email)
            .WithErrorMessage(
                "Адрес электронной почты имеет некорректный формат.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEmailExceedsMaximumLength()
    {
        // Arrange
        var domainLength = User.MaxEmailLength - "@example.com".Length + 1;
        var email = $"{new string('a', domainLength)}@example.com";

        var command = CreateValidCommand() with
        {
            Email = email
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Email)
            .WithErrorMessage(
                $"Адрес электронной почты не должен превышать " +
                $"{User.MaxEmailLength} символов.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenDisplayNameIsEmpty(
        string displayName)
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            DisplayName = displayName
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.DisplayName)
            .WithErrorMessage("Отображаемое имя обязательно.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDisplayNameExceedsMaximumLength()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            DisplayName = new string('a', User.MaxDisplayNameLength + 1)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.DisplayName)
            .WithErrorMessage(
                $"Отображаемое имя не должно превышать " +
                $"{User.MaxDisplayNameLength} символов.");
    }

    [Fact]
    public void Validate_ShouldNotHaveDisplayNameError_WhenTrimmedNameHasMaximumLength()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            DisplayName =
                $"  {new string('a', User.MaxDisplayNameLength)}  "
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(
            command => command.DisplayName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenPasswordIsEmpty(string password)
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            Password = password
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Password)
            .WithErrorMessage("Пароль обязателен.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordIsTooShort()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            Password = "Aa1!aaa"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Password)
            .WithErrorMessage(
                $"Пароль должен содержать не менее " +
                $"{RegisterUserCommandValidator.MinPasswordLength} символов.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordIsTooLong()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            Password = new string(
                'a',
                RegisterUserCommandValidator.MaxPasswordLength + 1)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Password)
            .WithErrorMessage(
                $"Пароль не должен превышать " +
                $"{RegisterUserCommandValidator.MaxPasswordLength} символов.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordHasNoUppercaseLetter()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            Password = "password1!"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Password)
            .WithErrorMessage(
                "Пароль должен содержать хотя бы одну заглавную букву.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordHasNoLowercaseLetter()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            Password = "PASSWORD1!"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Password)
            .WithErrorMessage(
                "Пароль должен содержать хотя бы одну строчную букву.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordHasNoDigit()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            Password = "Password!"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Password)
            .WithErrorMessage(
                "Пароль должен содержать хотя бы одну цифру.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordHasNoSpecialCharacter()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            Password = "Password1"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Password)
            .WithErrorMessage(
                "Пароль должен содержать хотя бы один специальный символ.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordContainsWhitespace()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            Password = "Password 1!"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Password)
            .WithErrorMessage(
                "Пароль не должен содержать пробельные символы.");
    }

    private static RegisterUserCommand CreateValidCommand()
    {
        return new RegisterUserCommand(
            Email: "user@example.com",
            DisplayName: "Иван",
            Password: "Password1!");
    }
}