using System.Text.Json;
using PlayersWebApp.Exceptions;

namespace PlayersWebApp.Middleware;

public class MyExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public MyExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
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
            var err = new ErrorDto { ErrType = "bad-deck-length"};
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
