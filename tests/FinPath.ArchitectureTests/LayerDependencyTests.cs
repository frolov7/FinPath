using System.Reflection;
using FinPath.Domain.Accounts;

namespace FinPath.ArchitectureTests
{
    public sealed class LayerDependencyTests
    {
        private static readonly Assembly DomainAssembly =
            typeof(Account).Assembly;

        private static readonly Assembly ApplicationAssembly =
            typeof(FinPath.Application.DependencyInjection).Assembly;

        private static readonly Assembly InfrastructureAssembly =
            typeof(FinPath.Infrastructure.DependencyInjection).Assembly;

        [Fact]
        public void Domain_ShouldNotDependOnOtherApplicationLayers()
        {
            AssertDoesNotReference(
                DomainAssembly,
                "FinPath.Application",
                "FinPath.Infrastructure",
                "FinPath.Api");
        }

        [Fact]
        public void Domain_ShouldNotDependOnEntityFrameworkCore()
        {
            AssertDoesNotReference(
                DomainAssembly,
                "Microsoft.EntityFrameworkCore");
        }

        [Fact]
        public void Domain_ShouldNotDependOnAspNetCore()
        {
            AssertDoesNotReference(
                DomainAssembly,
                "Microsoft.AspNetCore");
        }

        [Fact]
        public void Application_ShouldNotDependOnInfrastructureOrApi()
        {
            AssertDoesNotReference(
                ApplicationAssembly,
                "FinPath.Infrastructure",
                "FinPath.Api");
        }

        [Fact]
        public void Application_ShouldNotDependOnEntityFrameworkCore()
        {
            AssertDoesNotReference(
                ApplicationAssembly,
                "Microsoft.EntityFrameworkCore");
        }

        [Fact]
        public void Application_ShouldNotDependOnAspNetCore()
        {
            AssertDoesNotReference(
                ApplicationAssembly,
                "Microsoft.AspNetCore");
        }

        [Fact]
        public void Infrastructure_ShouldNotDependOnApi()
        {
            AssertDoesNotReference(
                InfrastructureAssembly,
                "FinPath.Api");
        }

        private static void AssertDoesNotReference(
            Assembly assembly,
            params string[] forbiddenAssemblyNames)
        {
            var referencedAssemblyNames = assembly
                .GetReferencedAssemblies()
                .Select(reference =>
                    reference.Name ?? string.Empty)
                .ToArray();

            var invalidReferences = referencedAssemblyNames
                .Where(referenceName =>
                    forbiddenAssemblyNames.Any(
                        forbiddenName =>
                            referenceName.Equals(
                                forbiddenName,
                                StringComparison.Ordinal)
                            || referenceName.StartsWith(
                                $"{forbiddenName}.",
                                StringComparison.Ordinal)))
                .OrderBy(referenceName => referenceName)
                .ToArray();

            Assert.True(
                invalidReferences.Length == 0,
                $"Сборка '{assembly.GetName().Name}' содержит " +
                $"запрещённые зависимости: " +
                $"{string.Join(", ", invalidReferences)}.");
        }
    }
}