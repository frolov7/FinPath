namespace FinPath.Application.Common.Interfaces;

/// <summary>
/// Предоставляет безопасное хеширование и проверку паролей.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Создаёт безопасный хеш пароля.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Проверяет соответствие пароля сохранённому хешу.
    /// </summary>
    bool VerifyPassword(string passwordHash, string password);
}