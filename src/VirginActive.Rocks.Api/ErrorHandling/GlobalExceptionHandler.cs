using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VirginActive.Rocks.Domain.Exceptions;

namespace VirginActive.Rocks.Api.ErrorHandling
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = exception switch
            {
                RockValidationException validationException => CreateValidationProblem(httpContext, validationException),

                RockNotFoundException notFoundException => CreateProblem(
                    httpContext,
                    StatusCodes.Status404NotFound,
                    "Rock not found",
                    notFoundException.Message),

                InvalidRockStateTransitionException transitionException => CreateProblem(
                    httpContext,
                    StatusCodes.Status422UnprocessableEntity,
                    "Invalid Rock state transition",
                    transitionException.Message),

                _ => CreateProblem(
                    httpContext,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred",
                    "An unexpected error occurred while processing the request.")
            };

            if (problemDetails.Status >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception while processing request {RequestPath}",
                    httpContext.Request.Path);
            }
            else
            {
                _logger.LogWarning(
                    "Request failed with status {StatusCode}. Error {ErrorType}",
                    problemDetails.Status,
                    exception.GetType().Name);
            }

            httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private static ProblemDetails CreateProblem(HttpContext context, int statusCode, string title, string detail)
        {
            return new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Extensions =
                {
                    ["correlationId"] = context.TraceIdentifier
                }
            };
        }

        private static ValidationProblemDetails CreateValidationProblem(HttpContext context, RockValidationException exception)
        {
            return new ValidationProblemDetails(exception.Errors.ToDictionary(x => x.Key, x => x.Value))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred.",
                Instance = context.Request.Path,
                Extensions =
                {
                    ["correlationId"] = context.TraceIdentifier
                }
            };
        }
    }
}
