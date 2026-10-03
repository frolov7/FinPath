using System.Security.Cryptography;
using FinPath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace FinPath.IntegrationTests.Common
{
    public sealed class FinPathWebApplicationFactory
        : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgreSqlContainer =
            new PostgreSqlBuilder("postgres:17-alpine")
                .WithDatabase("finpath_tests")
                .WithUsername("finpath")
                .WithPassword("finpath_test_password")
                .Build();

        private readonly string _tokenSecret;

        public FinPathWebApplicationFactory()
        {
            var secretBytes = new byte[64];

            RandomNumberGenerator.Fill(secretBytes);

            _tokenSecret =
                Convert.ToBase64String(secretBytes);

            Environment.SetEnvironmentVariable(
                "Jwt__Issuer",
                "FinPath.Api");

            Environment.SetEnvironmentVariable(
                "Jwt__Audience",
                "FinPath.Client");

            Environment.SetEnvironmentVariable(
                "Jwt__Secret",
                _tokenSecret);

            Environment.SetEnvironmentVariable(
                "Jwt__ExpirationMinutes",
                "30");
        }

        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<FinPathDbContext>();

                services.RemoveAll<
                    DbContextOptions<FinPathDbContext>>();

                services.RemoveAll<
                    IDbContextOptionsConfiguration<FinPathDbContext>>();

                services.AddDbContext<FinPathDbContext>(
                    options =>
                        options.UseNpgsql(
                            _postgreSqlContainer
                                .GetConnectionString()));
            });
        }

        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();

            using var scope = Services.CreateScope();

            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<FinPathDbContext>();

            await dbContext.Database.MigrateAsync();
        }

        public new async Task DisposeAsync()
        {
            await _postgreSqlContainer.DisposeAsync();
        }
    }
}