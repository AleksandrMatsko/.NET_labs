using MassTransit;
using PlayerLibrary;
using PlayersWebApp.Middleware;
using PlayersWebApp.RabbitMQ;
using SharedTransitLibrary;
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
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TellCardIndexConsumer>();
            x.AddConsumer<CardIndexToldConsumer>();
            
            x.UsingRabbitMq((context, conf) =>
            {
                conf.Host("localhost", "/", h =>
                {
                    h.Username("rmuser");
                    h.Password("rmpassword");
                });
                conf.ReceiveEndpoint("SharedTransitLibrary:CardIndexTold", e =>
                {
                    e.ConfigureConsumer<CardIndexToldConsumer>(context);
                });

                switch (_player.Name)
                {
                    case "Elon Mask":
                    {
                        conf.ReceiveEndpoint("Elon.SharedTransitLibrary.TellCardIndex", e =>
                        {
                            e.ConfigureConsumer<TellCardIndexConsumer>(context);
                        });
                        break;
                    }
                    case "Mark Zuckerberg":
                    {
                        conf.ReceiveEndpoint("Mark.SharedTransitLibrary.TellCardIndex", e =>
                        {
                            e.ConfigureConsumer<TellCardIndexConsumer>(context);
                        });
                        break;
                    }    
                }
            });
            services.AddSingleton<CardIndexToldService>();
            services.AddSingleton<ICardIndexToldHandler, CardIndexToldHandler>();
            services.AddSingleton<ITellCardIndexStorage, TellCardIndexStorage>();
        });
        Console.WriteLine($"{_player.Name}: services configured");
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMyExceptionMiddleware();
        
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
