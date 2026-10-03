namespace FinPath.Application.Common.Authentication
{
    /// <summary>
    /// Представляет созданный JWT-токен доступа.
    /// </summary>
    public sealed record AccessToken
    {
        /// <summary>
        /// Значение JWT-токена.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Дата и время окончания действия токена в UTC.
        /// </summary>
        public DateTimeOffset ExpiresAtUtc { get; }

        public AccessToken(string value, DateTimeOffset expiresAtUtc)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(
                    "Значение токена не может быть пустым.",
                    nameof(value));

            if (expiresAtUtc.Offset != TimeSpan.Zero)
                throw new ArgumentException(
                    "Дата окончания действия токена должна быть указана в UTC.",
                    nameof(expiresAtUtc));

            if (expiresAtUtc <= DateTimeOffset.UtcNow)
                throw new ArgumentOutOfRangeException(
                    nameof(expiresAtUtc),
                    "Дата окончания действия токена должна находиться в будущем.");

            Value = value;
            ExpiresAtUtc = expiresAtUtc;
        }
    }
}