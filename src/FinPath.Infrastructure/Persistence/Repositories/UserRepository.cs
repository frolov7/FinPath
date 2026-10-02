using FinPath.Application.Common.Interfaces;
using FinPath.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace FinPath.Infrastructure.Persistence.Repositories
{
    public sealed class UserRepository(FinPathDbContext context) : IUserRepository
    {
        private readonly FinPathDbContext _context = context;

        public Task AddAsync(User user, CancellationToken cancellationToken)
        {
            return _context.Users
                .AddAsync(user, cancellationToken).AsTask();
        }

        public Task<bool> ExistsByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        }

        public Task<User?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        }
    }
}
