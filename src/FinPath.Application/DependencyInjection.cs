using FinPath.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FinPath.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(
                    typeof(DependencyInjection).Assembly);

                configuration.AddOpenBehavior(
                    typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(
                typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}