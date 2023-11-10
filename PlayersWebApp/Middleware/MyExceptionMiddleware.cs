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
        catch (Exception e)
        {
            // TODO: catch custom exceptions, write custom responses
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
