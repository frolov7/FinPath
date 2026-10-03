using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using FinPath.Domain.Users;
using FinPath.Infrastructure.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FinPath.UnitTests.Authentication
{
    public sealed class JwtTokenGeneratorTests
    {
        private const string Issuer = "FinPath.Api";
        private const string Audience = "FinPath.Client";
        private const int ExpirationMinutes = 30;

        private readonly byte[] _secretBytes;
        private readonly JwtTokenGenerator _tokenGenerator;

        public JwtTokenGeneratorTests()
        {
            _secretBytes = new byte[64];

            RandomNumberGenerator.Fill(_secretBytes);

            var options = Options.Create(
                new JwtOptions
                {
                    Issuer = Issuer,
                    Audience = Audience,
                    Secret = Convert.ToBase64String(_secretBytes),
                    ExpirationMinutes = ExpirationMinutes
                });

            _tokenGenerator = new JwtTokenGenerator(options);
        }

        [Fact]
        public void Generate_ShouldReturnAccessToken_WhenUserIsValid()
        {
            // Arrange
            var user = CreateUser();

            var expectedMinimumExpiration =
                DateTimeOffset.UtcNow.AddMinutes(ExpirationMinutes);

            // Act
            var accessToken = _tokenGenerator.Generate(user);

            var expectedMaximumExpiration =
                DateTimeOffset.UtcNow.AddMinutes(ExpirationMinutes);

            // Assert
            Assert.False(
                string.IsNullOrWhiteSpace(accessToken.Value));

            Assert.InRange(
                accessToken.ExpiresAtUtc,
                expectedMinimumExpiration,
                expectedMaximumExpiration);

            Assert.Equal(
                TimeSpan.Zero,
                accessToken.ExpiresAtUtc.Offset);
        }

        [Fact]
        public void Generate_ShouldIncludeExpectedClaims_WhenUserIsValid()
        {
            // Arrange
            var user = CreateUser();

            // Act
            var accessToken = _tokenGenerator.Generate(user);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(
                accessToken.Value);

            // Assert
            Assert.Equal(Issuer, jwt.Issuer);

            Assert.Contains(
                Audience,
                jwt.Audiences);

            Assert.Equal(
                user.Id.ToString(),
                jwt.Claims.Single(
                    claim =>
                        claim.Type == JwtRegisteredClaimNames.Sub).Value);

            Assert.Equal(
                user.Email,
                jwt.Claims.Single(
                    claim =>
                        claim.Type == JwtRegisteredClaimNames.Email).Value);

            Assert.Equal(
                user.DisplayName,
                jwt.Claims.Single(
                    claim =>
                        claim.Type == JwtRegisteredClaimNames.Name).Value);

            Assert.False(
                string.IsNullOrWhiteSpace(
                    jwt.Claims.Single(
                        claim =>
                            claim.Type ==
                            JwtRegisteredClaimNames.Jti).Value));

            Assert.False(
                string.IsNullOrWhiteSpace(
                    jwt.Claims.Single(
                        claim =>
                            claim.Type ==
                            JwtRegisteredClaimNames.Iat).Value));
        }

        [Fact]
        public void Generate_ShouldUseHmacSha256Signature_WhenUserIsValid()
        {
            // Arrange
            var user = CreateUser();

            // Act
            var accessToken = _tokenGenerator.Generate(user);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(
                accessToken.Value);

            // Assert
            Assert.Equal(
                SecurityAlgorithms.HmacSha256,
                jwt.Header.Alg);

            Assert.False(
                string.IsNullOrWhiteSpace(jwt.RawSignature));
        }

        [Fact]
        public void Generate_ShouldCreateTokenWithValidSignature_WhenUserIsValid()
        {
            // Arrange
            var user = CreateUser();

            var accessToken = _tokenGenerator.Generate(user);

            var validationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,

                    ValidateAudience = true,
                    ValidAudience = Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(_secretBytes),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

            var handler = new JwtSecurityTokenHandler();

            // Act
            var principal = handler.ValidateToken(
                accessToken.Value,
                validationParameters,
                out var validatedToken);

            // Assert
            Assert.NotNull(principal);
            Assert.NotNull(validatedToken);
            Assert.IsType<JwtSecurityToken>(validatedToken);
        }

        [Fact]
        public void Generate_ShouldSetExpectedExpiration_WhenUserIsValid()
        {
            // Arrange
            var user = CreateUser();

            var issuedAfterUtc = DateTimeOffset.UtcNow;

            // Act
            var accessToken = _tokenGenerator.Generate(user);

            var jwt = new JwtSecurityTokenHandler()
                .ReadJwtToken(accessToken.Value);

            // Assert
            var expectedMinimumExpiration =
                issuedAfterUtc.AddMinutes(ExpirationMinutes);

            var expectedMaximumExpiration =
                DateTimeOffset.UtcNow.AddMinutes(ExpirationMinutes);

            Assert.InRange(
                accessToken.ExpiresAtUtc,
                expectedMinimumExpiration,
                expectedMaximumExpiration);

            var jwtExpirationUtc =
                new DateTimeOffset(jwt.ValidTo, TimeSpan.Zero);

            var difference =
                (accessToken.ExpiresAtUtc - jwtExpirationUtc).Duration();

            Assert.True(
                difference < TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Generate_ShouldThrowArgumentNullException_WhenUserIsNull()
        {
            // Arrange
            User? user = null;

            // Act
            var exception =
                Assert.Throws<ArgumentNullException>(
                    () => _tokenGenerator.Generate(user!));

            // Assert
            Assert.Equal("user", exception.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenOptionsAreNull()
        {
            // Arrange
            IOptions<JwtOptions>? options = null;

            // Act
            var exception =
                Assert.Throws<ArgumentNullException>(
                    () => new JwtTokenGenerator(options!));

            // Assert
            Assert.Equal("jwtOptions", exception.ParamName);
        }

        private static User CreateUser()
        {
            return User.Create(
                email: "user@example.com",
                displayName: "Иван",
                passwordHash: "stored-password-hash");
        }
    }
}