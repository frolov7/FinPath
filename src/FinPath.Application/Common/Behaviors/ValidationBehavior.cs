using FluentValidation;
using MediatR;

namespace FinPath.Application.Common.Behaviors
{
    /// <summary>
    /// Запускает FluentValidation перед выполнением обработчика запроса.
    /// </summary>
    public sealed class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IReadOnlyCollection<IValidator<TRequest>>
            _validators;

        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators.ToArray();
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_validators.Count == 0)
            {
                return await next(cancellationToken);
            }

            var validationContext =
                new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(validator =>
                    validator.ValidateAsync(
                        validationContext,
                        cancellationToken)));

            var validationFailures = validationResults
                .SelectMany(result => result.Errors)
                .Where(failure => failure is not null)
                .ToArray();

            if (validationFailures.Length > 0)
            {
                throw new ValidationException(
                    validationFailures);
            }

            return await next(cancellationToken);
        }
    }
}