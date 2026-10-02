namespace FinPath.Application.Common.Interfaces;

/// <summary>
/// Управляет сохранением изменений в базе данных
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Сохраняет накопленные изменения
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}