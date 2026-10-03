using FinPath.Api.Contracts.Authentication.Register;
using FinPath.Application.Common.Interfaces;
using FinPath.Infrastructure.Persistence;
using FinPath.IntegrationTests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace FinPath.IntegrationTests.Authentication
{
    public sealed class RegisterUserEndpointTests
        : IClassFixture<FinPathWebApplicationFactory>
    {
        private readonly FinPathWebApplicationFactory _factory;
        private readonly HttpClient _httpClient;

        public RegisterUserEndpointTests(
            FinPathWebApplicationFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ShouldReturnCreated_WhenRequestIsValid()
        {
            // Arrange
            var email = $"user-{Guid.NewGuid()}@example.com";
            const string displayName = "Yevgeniy";
            const string password = "Password1!";

            var request = new RegisterUserRequest
            {
                Email = email,
                DisplayName = displayName,
                Password = password
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync(
                "/api/v1/auth/register",
                request);

            // Assert: HTTP-ответ
            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            Assert.NotNull(response.Headers.Location);

            var responseBody =
                await response.Content
                    .ReadFromJsonAsync<RegisterUserResponse>();

            Assert.NotNull(responseBody);
            Assert.NotEqual(Guid.Empty, responseBody.Id);
            Assert.Equal(email, responseBody.Email);
            Assert.Equal(displayName, responseBody.DisplayName);

            Assert.Equal(
                $"/api/v1/users/{responseBody.Id}",
                response.Headers.Location.OriginalString);

            // Assert: запись в PostgreSQL
            using var scope = _factory.Services.CreateScope();

            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<FinPathDbContext>();

            var user = await dbContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    user =>
                        user.NormalizedEmail ==
                        email.ToUpperInvariant());

            Assert.NotNull(user);
            Assert.Equal(responseBody.Id, user.Id);
            Assert.Equal(email, user.Email);
            Assert.Equal(email.ToUpperInvariant(), user.NormalizedEmail);
            Assert.Equal(displayName, user.DisplayName);
            Assert.NotEqual(password, user.PasswordHash);

            var passwordHasher =
                scope.ServiceProvider
                    .GetRequiredService<IPasswordHasher>();

            Assert.True(
                passwordHasher.VerifyPassword(
                    user.PasswordHash,
                    password));
        }

        [Fact]
        public async Task Register_ShouldReturnConflict_WhenEmailAlreadyExists()
        {
            // Arrange
            var email = $"duplicate-{Guid.NewGuid()}@example.com";

            var firstRequest = new RegisterUserRequest
            {
                Email = email,
                DisplayName = "Первый пользователь",
                Password = "Password1!"
            };

            var secondRequest = new RegisterUserRequest
            {
                Email = email.ToUpperInvariant(),
                DisplayName = "Второй пользователь",
                Password = "Password2!"
            };

            // Act
            var firstResponse =
                await _httpClient.PostAsJsonAsync(
                    "/api/v1/auth/register",
                    firstRequest);

            var secondResponse =
                await _httpClient.PostAsJsonAsync(
                    "/api/v1/auth/register",
                    secondRequest);

            // Assert
            Assert.Equal(
                HttpStatusCode.Created,
                firstResponse.StatusCode);

            Assert.Equal(
                HttpStatusCode.Conflict,
                secondResponse.StatusCode);

            var problemDetails =
                await secondResponse.Content
                    .ReadFromJsonAsync<ProblemDetails>();

            Assert.NotNull(problemDetails);
            Assert.Equal(
                StatusCodes.Status409Conflict,
                problemDetails.Status);
            Assert.Equal(
                "Конфликт данных",
                problemDetails.Title);

            using var scope = _factory.Services.CreateScope();

            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<FinPathDbContext>();

            var usersCount = await dbContext.Users
                .AsNoTracking()
                .CountAsync(
                    user =>
                        user.NormalizedEmail ==
                        email.ToUpperInvariant());

            Assert.Equal(1, usersCount);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenEmailIsInvalid()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Email = "incorrect-email",
                DisplayName = "Yevgeniy",
                Password = "Password1!"
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync(
                "/api/v1/auth/register",
                request);

            // Assert
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);

            var problemDetails =
                await response.Content
                    .ReadFromJsonAsync<ValidationProblemDetails>();

            Assert.NotNull(problemDetails);
            Assert.Equal(
                StatusCodes.Status400BadRequest,
                problemDetails.Status);
            Assert.Equal(
                "Ошибка валидации",
                problemDetails.Title);
            Assert.Contains(
                "Email",
                problemDetails.Errors.Keys);

            using var scope = _factory.Services.CreateScope();

            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<FinPathDbContext>();

            var userExists = await dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.NormalizedEmail ==
                        request.Email.ToUpperInvariant());

            Assert.False(userExists);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenPasswordIsWeak()
        {
            // Arrange
            var email =
                $"weak-password-{Guid.NewGuid()}@example.com";

            var request = new RegisterUserRequest
            {
                Email = email,
                DisplayName = "Yevgeniy",
                Password = "123"
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync(
                "/api/v1/auth/register",
                request);

            // Assert
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);

            var problemDetails =
                await response.Content
                    .ReadFromJsonAsync<ValidationProblemDetails>();

            Assert.NotNull(problemDetails);
            Assert.Equal(
                StatusCodes.Status400BadRequest,
                problemDetails.Status);
            Assert.Equal(
                "Ошибка валидации",
                problemDetails.Title);
            Assert.Contains(
                "Password",
                problemDetails.Errors.Keys);

            using var scope = _factory.Services.CreateScope();

            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<FinPathDbContext>();

            var userExists = await dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.NormalizedEmail ==
                        email.ToUpperInvariant());

            Assert.False(userExists);
        }
    }
}