using Colosseum.Abstractions;

namespace PlayersWebApp;

public class Startup
{
    private readonly AbstractPlayer _player;

    public Startup(AbstractPlayer player)
    {
        _player = player;
    }
    
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSingleton(_player);
        Console.WriteLine($"{_player.Name}: services configured");
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoint =>
        {
            endpoint.MapControllers();
        });
        
        Console.WriteLine($"{_player.Name}: configured");
    }
}