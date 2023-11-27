using MassTransit;
using SharedTransitLibrary;

namespace PlayersWebApp;

public class WebStarter
{
    private static IHostBuilder CreateBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup(w => new Startup(w));
        });
    }
    
    public static void Main(string[] args)
    {
        var app = CreateBuilder(args).Build();
        app.Run();
    } 
}

