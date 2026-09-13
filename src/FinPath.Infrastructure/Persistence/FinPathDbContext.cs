using Microsoft.EntityFrameworkCore;

namespace FinPath.Infrastructure.Persistence
{
    public sealed class FinPathDbContext : DbContext
    {
        public FinPathDbContext(DbContextOptions<FinPathDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinPathDbContext).Assembly);
        }
    }
}
