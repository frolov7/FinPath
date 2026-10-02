using FinPath.Domain.Users;

namespace FinPath.UnitTests.Users;

/// <summary>
/// Содержит модульные тесты доменной сущности пользователя.
/// </summary>
public sealed class UserTests
{
    [Fact]
    public void Create_ShouldCreateUser_WhenArgumentsAreValid()
    {
        // Arrange
        var email = "user@example.com";
        var displayName = "Евгений Фролов";
        var passwordHash = "AQAAAAIAAYagAAAAEFakePasswordHash";
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var user = User.Create(
            email,
            displayName,
            passwordHash);

        var afterCreation = DateTimeOffset.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal("USER@EXAMPLE.COM", user.NormalizedEmail);
        Assert.Equal(displayName, user.DisplayName);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(user.CreatedAtUtc, user.UpdatedAtUtc);
        Assert.Equal(TimeSpan.Zero, user.CreatedAtUtc.Offset);
        Assert.InRange(
            user.CreatedAtUtc,
            beforeCreation,
            afterCreation);
    }

    [Fact]
    public void Create_ShouldTrimEmail_WhenEmailContainsOuterWhitespace()
    {
        // Arrange
        var email = "  User@Example.com  ";
        var displayName = "Евгений";
        var passwordHash = "AQAAAAIAAYagAAAAEFakePasswordHash";

        // Act
        var user = User.Create(
            email,
            displayName,
            passwordHash);

        // Assert
        Assert.Equal("User@Example.com", user.Email);
    }

    [Fact]
    public void Create_ShouldNormalizeEmail_WhenEmailUsesMixedCase()
    {
        // Arrange
        var email = "User@Example.com";
        var displayName = "Евгений";
        var passwordHash = "AQAAAAIAAYagAAAAEFakePasswordHash";

        // Act
        var user = User.Create(
            email,
            displayName,
            passwordHash);

        // Assert
        Assert.Equal("User@Example.com", user.Email);
        Assert.Equal("USER@EXAMPLE.COM", user.NormalizedEmail);
    }

    [Fact]
    public void Create_ShouldTrimDisplayName_WhenDisplayNameContainsOuterWhitespace()
    {
        // Arrange
        var email = "user@example.com";
        var displayName = "  Евгений Фролов  ";
        var passwordHash = "AQAAAAIAAYagAAAAEFakePasswordHash";

        // Act
        var user = User.Create(
            email,
            displayName,
            passwordHash);

        // Assert
        Assert.Equal("Евгений Фролов", user.DisplayName);
    }

    [Fact]
    public void Create_ShouldCreateUser_WhenEmailLengthEqualsMaximum()
    {
        // Arrange
        var email = new string('a', User.MaxEmailLength);
        var displayName = "Евгений";
        var passwordHash = "AQAAAAIAAYagAAAAEFakePasswordHash";

        // Act
        var user = User.Create(
            email,
            displayName,
            passwordHash);

        // Assert
        Assert.Equal(User.MaxEmailLength, user.Email.Length);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenEmailExceedsMaximumLength()
    {
        // Arrange
        var email = new string('a', User.MaxEmailLength + 1);
        var displayName = "Евгений";
        var passwordHash = "AQAAAAIAAYagAAAAEFakePasswordHash";

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            User.Create(
                email,
                displayName,
                passwordHash));

        // Assert
        Assert.Equal("email", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WhenEmailIsInvalid(
        string? invalidEmail)
    {
        // Arrange
        var displayName = "Евгений";
        var passwordHash = "AQAAAAIAAYagAAAAEFakePasswordHash";

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            User.Create(
                invalidEmail!,
                displayName,
                passwordHash));

        // Assert
        Assert.Equal("email", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldCreateUser_WhenDisplayNameLengthEqualsMaximum()
    {
        // Arrange
        var email = "user@example.com";
        var displayName = new string(
            'a',
            User.MaxDisplayNameLength);

        var passwordHash = "AQAAAAIAAYagAAAAEFakePasswordHash";

        // Act
        var user = User.Create(
            email,
            displayName,
            passwordHash);

        // Assert
        Assert.Equal(
            User.MaxDisplayNameLength,
            user.DisplayName.Length);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenDisplayNameExceedsMaximumLength()
    {
        // Arrange
        var email = "user@example.com";

        var displayName = new string(
            'a',
            User.MaxDisplayNameLength + 1);

        var passwordHash = "AQAAAAIAAYagAAAAEFakePasswordHash";

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            User.Create(
                email,
                displayName,
                passwordHash));

        // Assert
        Assert.Equal("displayName", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WhenDisplayNameIsInvalid(
        string? invalidDisplayName)
    {
        // Arrange
        var email = "user@example.com";
        var passwordHash = "AQAAAAIAAYagAAAAEFakePasswordHash";

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            User.Create(
                email,
                invalidDisplayName!,
                passwordHash));

        // Assert
        Assert.Equal("displayName", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WhenPasswordHashIsInvalid(
        string? invalidPasswordHash)
    {
        // Arrange
        var email = "user@example.com";
        var displayName = "Евгений";

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            User.Create(
                email,
                displayName,
                invalidPasswordHash!));

        // Assert
        Assert.Equal("passwordHash", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldPreservePasswordHash_WhenUserIsCreated()
    {
        // Arrange
        var email = "user@example.com";
        var displayName = "Евгений";
        var passwordHash = "AQAAAAIAAYagAAAAEFakePasswordHash==";

        // Act
        var user = User.Create(
            email,
            displayName,
            passwordHash);

        // Assert
        Assert.Equal(passwordHash, user.PasswordHash);
    }
}