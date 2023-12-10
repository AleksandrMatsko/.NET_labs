using System.Text.Json;
using PlayersWebApp.Exceptions;

namespace PlayersWebApp.Middleware;

public class MyExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MyExceptionMiddleware> _logger;

    public MyExceptionMiddleware(RequestDelegate next, ILogger<MyExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
        catch (BadDeckLength e)
        {
            context.Response.StatusCode = 400;
            var err = new ErrorDto { ErrType = "bad-deck-length" };
            var errResp = new ErrorResponse { Errors = new List<ErrorDto> { err } };
            await JsonSerializer.SerializeAsync(context.Response.Body, errResp);
        }
        catch (ExperimentNotFoundException e)
        {
            context.Response.StatusCode = 404;
            var err = new ErrorDto { ErrType = "experiment-not-found" };
            var errResp = new ErrorResponse { Errors = new List<ErrorDto> { err } };
            await JsonSerializer.SerializeAsync(context.Response.Body, errResp);
        }
        catch (InvalidIndexFromPartnerException e)
        {
            context.Response.StatusCode = 500;
            var err = new ErrorDto { ErrType = "invalid-index-from-partner" };
            var errResp = new ErrorResponse { Errors = new List<ErrorDto> { err } };
            await JsonSerializer.SerializeAsync(context.Response.Body, errResp);
        }
        catch (TimeoutException e)
        {   
            _logger.LogInformation("timeout exception");
            context.Response.StatusCode = 500;
            var err = new ErrorDto { ErrType = "wait-message-timeout" };
            var errResp = new ErrorResponse { Errors = new List<ErrorDto> { err } };
            await JsonSerializer.SerializeAsync(context.Response.Body, errResp);
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 500;
            var err = new ErrorDto { ErrType = "internal-error"};
            var errResp = new ErrorResponse { Errors = new List<ErrorDto> { err } };
            await JsonSerializer.SerializeAsync(context.Response.Body, errResp);
        } 
    }
}

public static class MyExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseMyExceptionMiddleware(
        this IApplicationBuilder builder)
    {
        Console.WriteLine("Use custom middleware");
        return builder.UseMiddleware<MyExceptionMiddleware>();
    }
}
