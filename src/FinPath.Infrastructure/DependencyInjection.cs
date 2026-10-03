using FinPath.Application.Common.Interfaces;
using FinPath.Infrastructure.Authentication;
using FinPath.Infrastructure.Persistence;
using FinPath.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FinPath.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("Database")
                ?? throw new InvalidOperationException(
                    "Connection string 'Database' not found.");

            var jwtSection =
                configuration.GetSection(JwtOptions.SectionName);

            var jwtOptions =
                jwtSection.Get<JwtOptions>()
                ?? throw new InvalidOperationException(
                    "Секция конфигурации JWT не найдена.");

            if (string.IsNullOrWhiteSpace(jwtOptions.Issuer))
            {
                throw new InvalidOperationException(
                    "JWT issuer не настроен.");
            }

            if (string.IsNullOrWhiteSpace(jwtOptions.Audience))
            {
                throw new InvalidOperationException(
                    "JWT audience не настроена.");
            }

            if (string.IsNullOrWhiteSpace(jwtOptions.Secret))
            {
                throw new InvalidOperationException(
                    "JWT secret не настроен.");
            }

            if (jwtOptions.ExpirationMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "Срок действия JWT должен быть больше нуля.");
            }

            byte[] secretBytes;

            try
            {
                secretBytes = Convert.FromBase64String(
                    jwtOptions.Secret);
            }
            catch (FormatException exception)
            {
                throw new InvalidOperationException(
                    "JWT secret должен иметь формат Base64.",
                    exception);
            }

            if (secretBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT secret должен содержать не менее 32 байт.");
            }

            services.AddDbContext<FinPathDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IUnitOfWork>(provider =>
                provider.GetRequiredService<FinPathDbContext>());

            services.Configure<JwtOptions>(jwtSection);

            services.AddSingleton<
                IJwtTokenGenerator,
                JwtTokenGenerator>();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.MapInboundClaims = false;

                    jwtBearerOptions.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = jwtOptions.Issuer,

                            ValidateAudience = true,
                            ValidAudience = jwtOptions.Audience,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    secretBytes),

                            ValidateLifetime = true,
                            RequireExpirationTime = true,

                            ClockSkew = TimeSpan.Zero
                        };
                });

            return services;
        }
    }
}