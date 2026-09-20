using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace FinPath.Domain.Accounts
{
    /// <summary>
    /// Представляет валюту, идентифицируемую трехбуквенным кодом ISO 4217.
    /// </summary>
    /// <remarks>Код должен соответствовать формату ISO 4217 (три буквенных символа). Не содержит информации о
    /// курсах обмена или локали.</remarks>
    public sealed record Currency
    {
        private static readonly HashSet<string> SupportedCodes = new(StringComparer.OrdinalIgnoreCase)
        {
            "USD",
            "EUR",
            "RUB",
            "KZT"
        };

        /// <summary>
        /// Трехбуквенный код валюты в формате ISO
        /// </summary>
        public string Code { get; }

        private Currency(string code)
        {
            Code = code;
        }

        /// <summary>
        /// Создает валюту из переданного кода.
        /// </summary>
        /// <param name="code">Трехбуквенный код валюты.</param>
        /// <returns>Созданный объект валюты.</returns>
        /// <exception cref="ArgumentException">
        /// Возникает, если код пустой, имеет неправильный формат
        /// или не поддерживается приложением.
        /// </exception>
        public static Currency Create(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Код валюты не может быть пустым.", nameof(code));

            var normalizedCode = code.Trim().ToUpperInvariant();

            if (normalizedCode.Length != 3)
                throw new ArgumentException("Код валюты должен состоять ровно из трех символов.", nameof(code));

            if (!normalizedCode.All(character => character is >= 'A' and <= 'Z'))
                throw new ArgumentException("Код валюты должен содержать только латинские буквы от A до Z.", nameof(code));

            if (!SupportedCodes.Contains(normalizedCode))
                throw new ArgumentException($"Код валюты '{normalizedCode}' не поддерживается приложением.", nameof(code));

            return new Currency(normalizedCode);
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
