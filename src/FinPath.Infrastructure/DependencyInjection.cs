using FinPath.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FinPath.Application.Common.Interfaces;
using FinPath.Infrastructure.Authentication;
using FinPath.Infrastructure.Persistence.Repositories;

namespace FinPath.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("Connection string 'Database' not found.");

            services.AddDbContext<FinPathDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork>(provider => 
                provider.GetRequiredService<FinPathDbContext>());

            return services;
        }
    }
}
