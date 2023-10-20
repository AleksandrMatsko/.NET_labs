using Colosseum.Abstractions;
using Colosseum.Impl;
using StrategyLibrary.Impl;

namespace PlayersWebApp;

public class Startup
{
    private readonly AbstractPlayer _player;

    public Startup(string playerName)
    {
        switch (playerName)
        {
            case "Elon":
            {
                _player = new ElonMask(new PickLastCardStrategy());
                break;
            }
            case "Mark": {
                _player = new MarkZuckerberg(new PickLastCardStrategy());
                break;
            }
            default: throw new ArgumentException("bad player name");
        }
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
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseRouting();
        app.UseEndpoints(endpoint =>
        {
            endpoint.MapControllers();
        });
        
        Console.WriteLine($"{_player.Name}: configured");
    }
}