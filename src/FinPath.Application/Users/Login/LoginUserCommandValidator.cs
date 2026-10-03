using FinPath.Domain.Users;
using FluentValidation;

namespace FinPath.Application.Users.Login
{
    public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command.Email)
                .NotEmpty()
                .WithMessage(
                    "Адрес электронной почты обязателен.")
                .Must(email =>
                    email.Trim().Length <= User.MaxEmailLength)
                .WithMessage(
                    $"Адрес электронной почты не должен превышать " +
                    $"{User.MaxEmailLength} символов.")
                .EmailAddress()
                .WithMessage(
                    "Адрес электронной почты имеет некорректный формат.");

            RuleFor(command => command.Password)
                .NotEmpty()
                .WithMessage(
                    "Пароль обязателен.")
                .Must(password =>
                    !string.IsNullOrWhiteSpace(password))
                .WithMessage(
                    "Пароль обязателен.");
        }
    }
}