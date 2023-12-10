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
    private readonly WebHostBuilderContext _builderContext;

    public Startup(WebHostBuilderContext context)
    {
        _builderContext = context;
        _player = _builderContext.Configuration["PLAYER"]! switch
        {
            "Elon" => new ElonMask(new PickLastCardStrategy()),
            "Mark" => new MarkZuckerberg(new PickLastCardStrategy()),
            _ => throw new ArgumentException("bad player name")
        };

        foreach (var c in _builderContext.Configuration.AsEnumerable())
        {
            Console.WriteLine($"{c.Key} = {c.Value}");
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
                conf.Host(
                    _builderContext.Configuration["RabbitMQ:host"]!, 
                    _builderContext.Configuration["RabbitMQ:virtualHost"]!, 
                    h =>
                {
                    h.Username(_builderContext.Configuration["RabbitMQ:user"]!);
                    h.Password(_builderContext.Configuration["RabbitMQ:password"]!);
                });
                conf.ReceiveEndpoint(_builderContext.Configuration["PLAYER"]! + "PublishQueue", e =>
                {
                    e.PurgeOnStartup = true;
                    e.ConfigureConsumer<CardIndexToldConsumer>(context);
                });
                
                conf.ReceiveEndpoint(_builderContext.Configuration["PLAYER"]! + ":" + 
                                     _builderContext.Configuration.GetConnectionString("selfQueue"),
                    e =>
                {
                    e.PurgeOnStartup = true;
                    e.ConfigureConsumer<TellCardIndexConsumer>(context);
                });
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
