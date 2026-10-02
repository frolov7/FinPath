namespace FinPath.Domain.Users;

/// <summary>
/// Представляет пользователя системы FinPath
/// </summary>
public sealed class User
{
    /// <summary>
    /// Максимальная длина адреса электронной почты
    /// </summary>
    public const int MaxEmailLength = 320;

    /// <summary>
    /// Максимальная длина отображаемого имени
    /// </summary>
    public const int MaxDisplayNameLength = 100;

    /// <summary>
    /// Уникальный идентификатор пользователя
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Адрес электронной почты пользователя
    /// </summary>
    public string Email { get; } = null!;

    /// <summary>
    /// Нормализованный email для поиска и проверки уникальности
    /// </summary>
    public string NormalizedEmail { get; } = null!;

    /// <summary>
    /// Отображаемое имя пользователя
    /// </summary>
    public string DisplayName { get; } = null!;

    /// <summary>
    /// Безопасный хеш пароля пользователя
    /// </summary>
    public string PasswordHash { get; } = null!;

    /// <summary>
    /// Дата создания пользователя в UTC
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; }

    /// <summary>
    /// Дата последнего изменения пользователя в UTC
    /// </summary>
    public DateTimeOffset UpdatedAtUtc { get; }

    /// <summary>
    /// Максимально допустимая длина хеша пароля в байтах.
    /// </summary>
    public const int MaxPasswordHashLength = 512;

    private User()
    {
    }

    /// <summary>
    /// Создает пользователя из подготовленных значений
    /// </summary>
    private User(
        Guid id,
        string email,
        string normalizedEmail,
        string displayName,
        string passwordHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc)
    {
        Id = id;
        Email = email;
        NormalizedEmail = normalizedEmail;
        DisplayName = displayName;
        PasswordHash = passwordHash;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Создает нового пользователя FinPath.
    /// </summary>
    /// <param name="email">Адрес электронной почты.</param>
    /// <param name="displayName">Отображаемое имя.</param>
    /// <param name="passwordHash">Безопасный хеш пароля.</param>
    /// <returns>Созданный пользователь.</returns>
    /// <exception cref="ArgumentException">
    /// Возникает, если переданные данные некорректны.
    /// </exception>
    public static User Create(string email, string displayName, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Адрес электронной почты обязателен.", nameof(email));

        var cleanEmail = email.Trim();

        if (cleanEmail.Length > MaxEmailLength)
            throw new ArgumentException($"Адрес электронной почты не должен превышать {MaxEmailLength} символов.", nameof(email));

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Отображаемое имя обязательно.", nameof(displayName));

        var trimmedDisplayName = displayName.Trim();

        if (trimmedDisplayName.Length > MaxDisplayNameLength)
            throw new ArgumentException($"Отображаемое имя не должно превышать {MaxDisplayNameLength} символов.", nameof(displayName));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Хеш пароля обязателен.", nameof(passwordHash));

        var normalizedEmail = cleanEmail.ToUpperInvariant();
        var currentTimeUtc = DateTimeOffset.UtcNow;

        return new User(
            Guid.NewGuid(),
            cleanEmail,
            normalizedEmail,
            trimmedDisplayName,
            passwordHash,
            currentTimeUtc,
            currentTimeUtc);
    }
}