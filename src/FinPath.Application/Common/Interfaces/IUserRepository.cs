using FinPath.Domain.Users;
namespace FinPath.Application.Common.Interfaces
{
    /// <summary>
    /// Предоставляет операции хранения и поиска пользователей.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Проверяет существование пользователя с указанным нормализованным email.
        /// </summary>
        Task<bool> ExistsByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает пользователя по нормализованному email.
        /// </summary>
        Task<User?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

        /// <summary>
        /// Добавляет нового пользователя.
        /// </summary>
        Task AddAsync(User user, CancellationToken cancellationToken);
    }
}
