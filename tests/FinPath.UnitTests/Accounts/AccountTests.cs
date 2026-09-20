using FinPath.Domain.Accounts;
using Xunit;

namespace FinPath.UnitTests.Accounts;

public sealed class AccountTests
{
    [Fact]
    public void Create_ShouldCreateAccount_WhenArgumentsAreValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Основная дебетовая карта";
        var type = AccountType.DebitCard;
        var currency = Currency.Create("RUB");
        var openingBalance = 150_000.50m;

        // Act
        var account = Account.Create(
            userId,
            name,
            type,
            currency,
            openingBalance);

        // Assert
        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.Equal(userId, account.UserId);
        Assert.Equal(name, account.Name);
        Assert.Equal(type, account.Type);
        Assert.Equal(currency, account.Currency);
        Assert.Equal(openingBalance, account.OpeningBalance);
        Assert.False(account.IsArchived);
        Assert.Equal(account.CreatedAtUtc, account.UpdatedAtUtc);
        Assert.Equal(TimeSpan.Zero, account.CreatedAtUtc.Offset);
    }

    [Fact]
    public void Create_ShouldTrimName_WhenNameContainsOuterWhitespace()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "  Основная дебетовая карта  ";
        var expectedName = "Основная дебетовая карта";
        var type = AccountType.DebitCard;
        var currency = Currency.Create("RUB");
        var openingBalance = 150_000.50m;

        // Act
        var account = Account.Create(
            userId,
            name,
            type,
            currency,
            openingBalance);

        // Assert
        Assert.Equal(expectedName, account.Name);
    }

    [Fact]
    public void Create_ShouldPreserveNameCasing_WhenAccountIsCreated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Основная Карта RUB";
        var type = AccountType.DebitCard;
        var currency = Currency.Create("RUB");
        var openingBalance = 150_000.50m;

        // Act
        var account = Account.Create(
            userId,
            name,
            type,
            currency,
            openingBalance);

        // Assert
        Assert.Equal(name, account.Name);
    }

    [Fact]
    public void Create_ShouldCreateAccount_WhenNameLengthEqualsMaximum()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = new string('A', Account.MaxNameLength);
        var type = AccountType.DebitCard;
        var currency = Currency.Create("RUB");
        var openingBalance = 150_000.50m;

        // Act
        var account = Account.Create(
            userId,
            name,
            type,
            currency,
            openingBalance);

        // Assert
        Assert.Equal(name, account.Name);
        Assert.Equal(Account.MaxNameLength, account.Name.Length);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = new string('A', Account.MaxNameLength + 1);
        var type = AccountType.DebitCard;
        var currency = Currency.Create("RUB");
        var openingBalance = 150_000.50m;

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Account.Create(
                userId,
                name,
                type,
                currency,
                openingBalance));

        // Assert
        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenUserIdIsEmpty()
    {
        // Arrange
        var userId = Guid.Empty;
        var name = "Основная дебетовая карта";
        var type = AccountType.DebitCard;
        var currency = Currency.Create("RUB");
        var openingBalance = 150_000.50m;

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Account.Create(
                userId,
                name,
                type,
                currency,
                openingBalance));

        // Assert
        Assert.Equal("userId", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("     ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_ShouldThrowArgumentException_WhenNameIsInvalid(string invalidName)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var type = AccountType.DebitCard;
        var currency = Currency.Create("RUB");
        var openingBalance = 150_000.50m;

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Account.Create(
                userId,
                invalidName,
                type,
                currency,
                openingBalance));

        // Assert
        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameIsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var type = AccountType.DebitCard;
        var currency = Currency.Create("RUB");
        var openingBalance = 150_000.50m;

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Account.Create(
                userId,
                null!,
                type,
                currency,
                openingBalance));

        // Assert
        Assert.Equal("name", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(999)]
    public void Create_ShouldThrowArgumentOutOfRangeException_WhenAccountTypeIsUndefined(
        int invalidTypeValue)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Основная дебетовая карта";
        var type = (AccountType)invalidTypeValue;
        var currency = Currency.Create("RUB");
        var openingBalance = 150_000.50m;

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            Account.Create(
                userId,
                name,
                type,
                currency,
                openingBalance));

        // Assert
        Assert.Equal("type", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentNullException_WhenCurrencyIsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Основная дебетовая карта";
        var type = AccountType.DebitCard;
        var openingBalance = 150_000.50m;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            Account.Create(
                userId,
                name,
                type,
                null!,
                openingBalance));

        // Assert
        Assert.Equal("currency", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldCreateAccount_WhenOpeningBalanceIsZero()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Основная дебетовая карта";
        var type = AccountType.DebitCard;
        var currency = Currency.Create("RUB");
        var openingBalance = 0m;

        // Act
        var account = Account.Create(
            userId,
            name,
            type,
            currency,
            openingBalance);

        // Assert
        Assert.NotNull(account);
        Assert.Equal(openingBalance, account.OpeningBalance);
        Assert.False(account.IsArchived);
    }

    [Fact]
    public void Create_ShouldCreateAccount_WhenOpeningBalanceIsNegative()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Основная дебетовая карта";
        var type = AccountType.DebitCard;
        var currency = Currency.Create("RUB");
        var openingBalance = -50_000m;

        // Act
        var account = Account.Create(
            userId,
            name,
            type,
            currency,
            openingBalance);

        // Assert
        Assert.NotNull(account);
        Assert.Equal(openingBalance, account.OpeningBalance);
    }

    [Fact]
    public void Create_ShouldGenerateDifferentIds_WhenMultipleAccountsAreCreated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Основная дебетовая карта";
        var type = AccountType.DebitCard;
        var currency = Currency.Create("RUB");
        var openingBalance = 50_000m;

        // Act
        var firstAccount = Account.Create(
            userId,
            name,
            type,
            currency,
            openingBalance);

        var secondAccount = Account.Create(
            userId,
            name,
            type,
            currency,
            openingBalance);

        // Assert
        Assert.NotEqual(Guid.Empty, firstAccount.Id);
        Assert.NotEqual(Guid.Empty, secondAccount.Id);
        Assert.NotEqual(firstAccount.Id, secondAccount.Id);
    }

    [Fact]
    public void Archive_ShouldArchiveAccount_WhenAccountIsActive()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Основная дебетовая карта";
        var type = AccountType.DebitCard;
        var currency = Currency.Create("RUB");
        var openingBalance = 50_000m;

        var account = Account.Create(
            userId,
            name,
            type,
            currency,
            openingBalance);

        var originalId = account.Id;
        var originalUserId = account.UserId;
        var originalName = account.Name;
        var originalType = account.Type;
        var originalCurrency = account.Currency;
        var originalOpeningBalance = account.OpeningBalance;
        var originalCreatedAtUtc = account.CreatedAtUtc;

        // Act
        account.Archive();

        // Assert
        Assert.True(account.IsArchived);
        Assert.True(account.UpdatedAtUtc >= account.CreatedAtUtc);

        Assert.Equal(originalId, account.Id);
        Assert.Equal(originalUserId, account.UserId);
        Assert.Equal(originalName, account.Name);
        Assert.Equal(originalType, account.Type);
        Assert.Equal(originalCurrency, account.Currency);
        Assert.Equal(originalOpeningBalance, account.OpeningBalance);
        Assert.Equal(originalCreatedAtUtc, account.CreatedAtUtc);
    }

    [Fact]
    public void Archive_ShouldNotChangeUpdatedAtUtc_WhenAccountIsAlreadyArchived()
    {
        // Arrange
        var account = Account.Create(
            Guid.NewGuid(),
            "Основная дебетовая карта",
            AccountType.DebitCard,
            Currency.Create("RUB"),
            50_000m);

        account.Archive();
        var updatedAtUtcAfterFirstArchive = account.UpdatedAtUtc;

        // Act
        account.Archive();

        // Assert
        Assert.True(account.IsArchived);
        Assert.Equal(
            updatedAtUtcAfterFirstArchive,
            account.UpdatedAtUtc);
    }
}