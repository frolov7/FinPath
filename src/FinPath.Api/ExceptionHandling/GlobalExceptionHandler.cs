using FinPath.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FinPath.Api.ExceptionHandling
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
        {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ValidationException validationException)
            {
                var errors = validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group
                            .Select(error => error.ErrorMessage)
                            .Distinct()
                            .ToArray());

                httpContext.Response.StatusCode =
                    StatusCodes.Status400BadRequest;

                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Запрос содержит некорректные данные.",
                    Instance = httpContext.Request.Path,
                    Title = "Ошибка валидации"
                };

                await _problemDetailsService.WriteAsync(
                    new ProblemDetailsContext
                    {
                        HttpContext = httpContext,
                        ProblemDetails = problemDetails
                    });

                return true;
            }
            else if (exception is ConflictException conflictException)
            {
                httpContext.Response.StatusCode =
                    StatusCodes.Status409Conflict;

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Конфликт данных",
                    Detail = conflictException.Message,
                    Instance = httpContext.Request.Path
                };

                await _problemDetailsService.WriteAsync(
                    new ProblemDetailsContext
                    {
                        HttpContext = httpContext,
                        ProblemDetails = problemDetails
                    });

                return true;
            }
            else
            {
                _logger.LogError(
                    exception,
                    "При обработке запроса {RequestPath} возникло " +
                    "необработанное исключение. TraceId: {TraceId}",
                    httpContext.Request.Path,
                    httpContext.TraceIdentifier);

                httpContext.Response.StatusCode =
                    StatusCodes.Status500InternalServerError;

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Внутренняя ошибка сервера",
                    Detail =
                        "Во время обработки запроса произошла непредвиденная ошибка.",
                    Instance = httpContext.Request.Path
                };

                problemDetails.Extensions["traceId"] =
                    httpContext.TraceIdentifier;

                await _problemDetailsService.WriteAsync(
                    new ProblemDetailsContext
                    {
                        HttpContext = httpContext,
                        ProblemDetails = problemDetails
                    });

                return true;
            }
        }
    }
}
