using Blackjack.Application.Games;
using Blackjack.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Blackjack.Api.Errors;

internal sealed class GameExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        int? status = exception switch
        {
            GameNotFoundException => StatusCodes.Status404NotFound,
            InvalidGameActionException => StatusCodes.Status409Conflict,
            DomainException => StatusCodes.Status400BadRequest,
            _ => null
        };

        if (status is null) return false;

        httpContext.Response.StatusCode = status.Value;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = { Status = status, Detail = exception.Message }
        });
    }
}