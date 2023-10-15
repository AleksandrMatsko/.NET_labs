namespace PlayersWebApp;

public class WebStarter
{
    private static IHostBuilder CreateBuilder()
    {
        return Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
    }
    
    public static void Main(string[] args)
    {
        var app = CreateBuilder().Build();
        app.Run();
    } 
}

