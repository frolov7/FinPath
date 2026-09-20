using FinPath.Domain.Accounts;
using Xunit;

namespace FinPath.UnitTests.Accounts;

public sealed class CurrencyTests
{
    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("RUB")]
    [InlineData("KZT")]
    public void Create_ShouldCreateCurrency_WhenCodeIsSupported(string supportedCode)
    {
        // Act
        var currency = Currency.Create(supportedCode);

        // Assert
        Assert.NotNull(currency);
        Assert.Equal(supportedCode, currency.Code);
    }

    [Theory]
    [InlineData("usd", "USD")]
    [InlineData("eur", "EUR")]
    [InlineData("rub", "RUB")]
    [InlineData("kzt", "KZT")]
    [InlineData("uSd", "USD")]
    [InlineData("RuB", "RUB")]
    public void Create_ShouldNormalizeCodeToUpperCase_WhenCodeContainsLowerCaseLetters(string code, string expectedCode)
    {
        // Act
        var currency = Currency.Create(code);

        // Assert
        Assert.Equal(expectedCode, currency.Code);
    }

    [Theory]
    [InlineData(" USD ", "USD")]
    [InlineData("  EUR  ", "EUR")]
    [InlineData("\tRUB\t", "RUB")]
    [InlineData("\nKZT\n", "KZT")]
    public void Create_ShouldTrimCode_WhenCodeContainsOuterWhitespace(string code, string expectedCode)
    {
        // Act
        var currency = Currency.Create(code);

        // Assert
        Assert.Equal(expectedCode, currency.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("     ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_ShouldThrowArgumentException_WhenCodeIsEmptyOrWhitespace(string invalidCode)
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Currency.Create(invalidCode));

        // Assert
        Assert.Equal("code", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenCodeIsNull()
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Currency.Create(null!));

        // Assert
        Assert.Equal("code", exception.ParamName);
    }

    [Theory]
    [InlineData("U")]
    [InlineData("US")]
    [InlineData("USDD")]
    [InlineData("RUBLE")]
    public void Create_ShouldThrowArgumentException_WhenCodeLengthIsInvalid(string invalidCode)
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Currency.Create(invalidCode));

        // Assert
        Assert.Equal("code", exception.ParamName);
    }

    [Theory]
    [InlineData("RU1")]
    [InlineData("R-B")]
    [InlineData("R_U")]
    [InlineData("R U")]
    [InlineData("РУБ")]
    [InlineData("€UR")]
    public void Create_ShouldThrowArgumentException_WhenCodeContainsInvalidCharacters(string invalidCode)
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Currency.Create(invalidCode));

        // Assert
        Assert.Equal("code", exception.ParamName);
    }

    [Theory]
    [InlineData("AAA")]
    [InlineData("JPY")]
    [InlineData("CHF")]
    [InlineData("AED")]
    public void Create_ShouldThrowArgumentException_WhenCodeIsNotSupported(string unsupportedCode)
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Currency.Create(unsupportedCode));

        // Assert
        Assert.Equal("code", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldReturnEqualCurrencies_WhenNormalizedCodesAreEqual()
    {
        // Arrange
        var firstCurrency = Currency.Create("RUB");
        var secondCurrency = Currency.Create(" rub ");

        // Assert
        Assert.Equal(firstCurrency, secondCurrency);
        Assert.Equal(firstCurrency.GetHashCode(), secondCurrency.GetHashCode());
    }

    [Fact]
    public void Create_ShouldReturnDifferentCurrencies_WhenCodesAreDifferent()
    {
        // Arrange
        var firstCurrency = Currency.Create("RUB");
        var secondCurrency = Currency.Create("USD");

        // Assert
        Assert.NotEqual(firstCurrency, secondCurrency);
    }

    [Theory]
    [InlineData("RUB")]
    [InlineData("USD")]
    [InlineData("EUR")]
    public void ToString_ShouldReturnCurrencyCode(string code)
    {
        // Arrange
        var currency = Currency.Create(code);

        // Act
        var result = currency.ToString();

        // Assert
        Assert.Equal(code, result);
    }
}