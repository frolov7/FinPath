namespace FinPath.Domain.Accounts;

/// <summary>
/// Представляет финансовый счет пользователя.
/// </summary>
/// <remarks>
/// Финансовым счетом может быть наличный кошелек, банковский счет,
/// дебетовая или кредитная карта, накопительный либо инвестиционный счет.
/// Изменение состояния счета выполняется только через доменные методы.
/// </remarks>
public sealed class Account
{
    /// <summary>
    /// Максимально допустимая длина названия финансового счета.
    /// </summary>
    public const int MaxNameLength = 100;

    /// <summary>
    ///Идентификатор финансового счета.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор пользователя, которому принадлежит счет.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Пользовательское название финансового счета.
    /// </summary>
    /// <example>Основная дебетовая карта.</example>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Тип финансового счета.
    /// </summary>
    public AccountType Type { get; private set; }

    /// <summary>
    /// Валюту финансового счета.
    /// </summary>
    public Currency Currency { get; private set; } = null!;

    /// <summary>
    /// Первоначальный баланс, указанный при создании счета.
    /// </summary>
    /// <remarks>
    /// Первоначальный баланс не является текущим балансом.
    /// Отрицательное значение разрешено и может обозначать существующую задолженность.
    /// </remarks>
    public decimal OpeningBalance { get; private set; }

    /// <summary>
    /// Признак того, что финансовый счет перемещен в архив.
    /// </summary>
    /// <remarks>
    /// Архивный счет нельзя будет использовать для создания новых финансовых операций.
    /// </remarks>
    public bool IsArchived { get; private set; }

    /// <summary>
    /// Дата и время создания финансового счета в формате UTC.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; private set; }

    /// <summary>
    /// Дата и время последнего изменения финансового счета в формате UTC.
    /// </summary>
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    private Account()
    {
    }

    private Account(Guid userId, string name, AccountType type, Currency currency, decimal openingBalance, DateTimeOffset createdAtUtc)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        Type = type;
        Currency = currency;
        OpeningBalance = openingBalance;
        IsArchived = false;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Создает новый финансовый счет после проверки бизнес-правил.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя — владельца счета.</param>
    /// <param name="name">Пользовательское название финансового счета.</param>
    /// <param name="type">Тип финансового счета.</param>
    /// <param name="currency">Валюта финансового счета.</param>
    /// <param name="openingBalance">Первоначальный баланс финансового счета.</param>
    /// <returns>Новый корректно инициализированный финансовый счет.</returns>
    /// <exception cref="ArgumentException">
    /// Возникает, если идентификатор пользователя пуст,
    /// название не заполнено или превышает допустимую длину.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Возникает, если передан неподдерживаемый тип финансового счета.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Возникает, если не передана валюта финансового счета.
    /// </exception>
    public static Account Create(Guid userId, string name, AccountType type, Currency currency, decimal openingBalance)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Идентификатор пользователя не может быть пустым.", nameof(userId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название финансового счета не может быть пустым.", nameof(name));

        var normalizedName = name.Trim();

        if (normalizedName.Length > MaxNameLength)
            throw new ArgumentException($"Название финансового счета не может превышать " + $"{MaxNameLength} символов.", nameof(name));

        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type), type, "Передан неподдерживаемый тип финансового счета.");

        ArgumentNullException.ThrowIfNull(currency);

        var createdAtUtc = DateTimeOffset.UtcNow;

        return new Account(
            userId,
            normalizedName,
            type,
            currency,
            openingBalance,
            createdAtUtc);
    }

    /// <summary>
    /// Перемещает финансовый счет в архив.
    /// </summary>
    /// <remarks>
    /// Операция является идемпотентной: повторный вызов для уже архивного
    /// счета не изменяет состояние и дату последнего обновления.
    /// </remarks>
    public void Archive()
    {
        if (IsArchived)
            return;

        IsArchived = true;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}