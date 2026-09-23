namespace Auth.Api.Middleware;

public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError("An error occured: {message}", exception);

        var (statusCode, title) = GetException(exception);
        var problemDetail = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetail, cancellationToken);

        return true;
    }

    private (int statusCode, string title) GetException(Exception exception)
    {
        return exception switch
        {
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ConflictException => (StatusCodes.Status409Conflict, "User already exists."),
            _ => (StatusCodes.Status500InternalServerError, "An internal server error occured. please try again later")
        };
    }
}