using FinPath.Domain.Accounts;
using FinPath.Domain.Users;
using FinPath.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinPath.Infrastructure.Persistence
{
    public sealed class FinPathDbContext : DbContext, IUnitOfWork
    {
        public FinPathDbContext(DbContextOptions<FinPathDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<User> Users => Set<User>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinPathDbContext).Assembly);
        }
    }
}
