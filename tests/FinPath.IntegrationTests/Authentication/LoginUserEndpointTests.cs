using FinPath.Api.Contracts.Authentication.Login;
using FinPath.Api.Contracts.Authentication.Register;
using FinPath.IntegrationTests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace FinPath.IntegrationTests.Authentication
{
    public sealed class LoginUserEndpointTests
        : IClassFixture<FinPathWebApplicationFactory>
    {
        private const string ValidPassword = "Password1!";

        private readonly HttpClient _httpClient;

        public LoginUserEndpointTests(
            FinPathWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var email = CreateUniqueEmail();

            await RegisterUserAsync(
                email,
                "Иван",
                ValidPassword);

            var request = new LoginUserRequest
            {
                Email = email,
                Password = ValidPassword
            };

            var expirationMinimum =
                DateTimeOffset.UtcNow;

            // Act
            var response = await _httpClient.PostAsJsonAsync(
                "/api/v1/auth/login",
                request);

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var result =
                await response.Content
                    .ReadFromJsonAsync<LoginUserResponse>();

            Assert.NotNull(result);

            Assert.False(
                string.IsNullOrWhiteSpace(
                    result.AccessToken));

            Assert.True(
                result.ExpiresAtUtc > expirationMinimum);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
        {
            // Arrange
            var email = CreateUniqueEmail();

            await RegisterUserAsync(
                email,
                "Иван",
                ValidPassword);

            var request = new LoginUserRequest
            {
                Email = email,
                Password = "IncorrectPassword1!"
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync(
                "/api/v1/auth/login",
                request);

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);

            var problemDetails =
                await response.Content
                    .ReadFromJsonAsync<ProblemDetails>();

            Assert.NotNull(problemDetails);

            Assert.Equal(
                StatusCodes.Status401Unauthorized,
                problemDetails.Status);

            Assert.Equal(
                "Ошибка аутентификации",
                problemDetails.Title);

            Assert.Equal(
                "Неверный адрес электронной почты или пароль.",
                problemDetails.Detail);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Email = CreateUniqueEmail(),
                Password = ValidPassword
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync(
                "/api/v1/auth/login",
                request);

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);

            var problemDetails =
                await response.Content
                    .ReadFromJsonAsync<ProblemDetails>();

            Assert.NotNull(problemDetails);

            Assert.Equal(
                StatusCodes.Status401Unauthorized,
                problemDetails.Status);

            Assert.Equal(
                "Ошибка аутентификации",
                problemDetails.Title);

            Assert.Equal(
                "Неверный адрес электронной почты или пароль.",
                problemDetails.Detail);
        }

        private async Task RegisterUserAsync(
            string email,
            string displayName,
            string password)
        {
            var request = new RegisterUserRequest
            {
                Email = email,
                DisplayName = displayName,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/api/v1/auth/register",
                request);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);
        }

        private static string CreateUniqueEmail()
        {
            return $"user-{Guid.NewGuid():N}@example.com";
        }
    }
}