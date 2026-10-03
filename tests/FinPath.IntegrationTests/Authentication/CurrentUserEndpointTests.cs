using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FinPath.Api.Contracts.Authentication.CurrentUser;
using FinPath.Api.Contracts.Authentication.Login;
using FinPath.Api.Contracts.Authentication.Register;
using FinPath.IntegrationTests.Common;

namespace FinPath.IntegrationTests.Authentication
{
    public sealed class CurrentUserEndpointTests
        : IClassFixture<FinPathWebApplicationFactory>
    {
        private const string ValidPassword = "Password1!";

        private readonly HttpClient _httpClient;

        public CurrentUserEndpointTests(
            FinPathWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnUser_WhenTokenIsValid()
        {
            // Arrange
            var email = CreateUniqueEmail();
            const string displayName = "Иван";

            await RegisterUserAsync(
                email,
                displayName,
                ValidPassword);

            var accessToken = await LoginUserAsync(
                email,
                ValidPassword);

            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/v1/auth/me");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    accessToken);

            // Act
            using var response =
                await _httpClient.SendAsync(request);

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var currentUser =
                await response.Content
                    .ReadFromJsonAsync<CurrentUserResponse>();

            Assert.NotNull(currentUser);

            Assert.NotEqual(
                Guid.Empty,
                currentUser.Id);

            Assert.Equal(
                email,
                currentUser.Email);

            Assert.Equal(
                displayName,
                currentUser.DisplayName);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenTokenIsMissing()
        {
            // Arrange
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/v1/auth/me");

            // Act
            using var response =
                await _httpClient.SendAsync(request);

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenTokenIsInvalid()
        {
            // Arrange
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/v1/auth/me");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    "invalid-access-token");

            // Act
            using var response =
                await _httpClient.SendAsync(request);

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
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

            using var response =
                await _httpClient.PostAsJsonAsync(
                    "/api/v1/auth/register",
                    request);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);
        }

        private async Task<string> LoginUserAsync(
            string email,
            string password)
        {
            var request = new LoginUserRequest
            {
                Email = email,
                Password = password
            };

            using var response =
                await _httpClient.PostAsJsonAsync(
                    "/api/v1/auth/login",
                    request);

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var loginResult =
                await response.Content
                    .ReadFromJsonAsync<LoginUserResponse>();

            Assert.NotNull(loginResult);

            Assert.False(
                string.IsNullOrWhiteSpace(
                    loginResult.AccessToken));

            return loginResult.AccessToken;
        }

        private static string CreateUniqueEmail()
        {
            return $"user-{Guid.NewGuid():N}@example.com";
        }
    }
}