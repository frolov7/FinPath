using FinPath.Domain.Accounts;
using Microsoft.EntityFrameworkCore;

namespace FinPath.Infrastructure.Persistence
{
    public sealed class FinPathDbContext : DbContext
    {
        public FinPathDbContext(DbContextOptions<FinPathDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts => Set<Account>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinPathDbContext).Assembly);
        }
    }
}
