using FinPath.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FinPath.Infrastructure.Authentication
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private static readonly object PasswordHasherUser = new();
        private readonly Microsoft.AspNetCore.Identity.PasswordHasher<object> _passwordHasher = new();

        public string HashPassword(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password), "Пароль не может быть null.");

            return _passwordHasher.HashPassword(PasswordHasherUser, password);
        }

        public bool VerifyPassword(string passwordHash, string password)
        {
            if (passwordHash == null)
                throw new ArgumentNullException(nameof(passwordHash), "Хеш пароля не может быть null.");

            if (password == null)
                throw new ArgumentNullException(nameof(password), "Пароль не может быть null.");

            var result = _passwordHasher.VerifyHashedPassword(
                PasswordHasherUser,
                passwordHash,
                password);

            return result is
                PasswordVerificationResult.Success or
                PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
