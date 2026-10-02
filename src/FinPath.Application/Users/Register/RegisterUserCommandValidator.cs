using FinPath.Domain.Users;
using FluentValidation;

namespace FinPath.Application.Users.Register
{
    public sealed class RegisterUserCommandValidator
        : AbstractValidator<RegisterUserCommand>
    {
        public const int MinPasswordLength = 8;
        public const int MaxPasswordLength = 128;

        public RegisterUserCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command.Email)
                .NotEmpty()
                .WithMessage("Адрес электронной почты обязателен.")
                .Must(email =>
                    email.Trim().Length <= User.MaxEmailLength)
                .WithMessage(
                    $"Адрес электронной почты не должен превышать " +
                    $"{User.MaxEmailLength} символов.")
                .EmailAddress()
                .WithMessage(
                    "Адрес электронной почты имеет некорректный формат.");

            RuleFor(command => command.DisplayName)
                .NotEmpty()
                .WithMessage("Отображаемое имя обязательно.")
                .Must(displayName =>
                    displayName.Trim().Length <=
                    User.MaxDisplayNameLength)
                .WithMessage(
                    $"Отображаемое имя не должно превышать " +
                    $"{User.MaxDisplayNameLength} символов.");

            RuleFor(command => command.Password)
                .NotEmpty()
                .WithMessage("Пароль обязателен.")
                .MinimumLength(MinPasswordLength)
                .WithMessage(
                    $"Пароль должен содержать не менее " +
                    $"{MinPasswordLength} символов.")
                .MaximumLength(MaxPasswordLength)
                .WithMessage(
                    $"Пароль не должен превышать " +
                    $"{MaxPasswordLength} символов.")
                .Must(password =>
                    password.All(character =>
                        !char.IsWhiteSpace(character)))
                .WithMessage(
                    "Пароль не должен содержать пробельные символы.")
                .Must(password =>
                    password.Any(char.IsUpper))
                .WithMessage(
                    "Пароль должен содержать хотя бы одну заглавную букву.")
                .Must(password =>
                    password.Any(char.IsLower))
                .WithMessage(
                    "Пароль должен содержать хотя бы одну строчную букву.")
                .Must(password =>
                    password.Any(char.IsDigit))
                .WithMessage(
                    "Пароль должен содержать хотя бы одну цифру.")
                .Must(password =>
                    password.Any(character =>
                        !char.IsLetterOrDigit(character) &&
                        !char.IsWhiteSpace(character)))
                .WithMessage(
                    "Пароль должен содержать хотя бы один специальный символ.");
        }
    }
}