using CardLibrary.Abstractions;
using CardLibrary.Impl;
using CardStorage;
using Colosseum.Experiments;
using Colosseum.Abstractions;
using Colosseum.Exceptions;
using Colosseum.RabbitMQ;
using Colosseum.Workers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlayerLibrary;
using SharedTransitLibrary;
using StrategyLibrary.Impl;

namespace Colosseum;

class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        switch (args.Length)
        {
            case 1:
            {
                var myConfig = new MyConfig {ExperimentCount = int.Parse(args[0]), Request = DbRequest.None};
                return Host.CreateDefaultBuilder(args)
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<ExperimentWorker>();
                        services.AddSingleton<IDeckShuffler, RandomDeckShuffler>();
                        services.AddScoped<IExperiment, SimpleExperiment>();
                        services.AddSingleton<AbstractPlayer>(new ElonMask(new PickFirstCardStrategy()));
                        services.AddSingleton<AbstractPlayer>(new MarkZuckerberg(new PickFirstCardStrategy()));
                        services.AddSingleton(myConfig);
                    });
            }
            case 3:
            {
                var myConfig = args[0] switch
                {
                    "generate" => new MyConfig { ExperimentCount = int.Parse(args[1]), Request = DbRequest.Generate },
                    "useGenerated" => new MyConfig { ExperimentCount = int.Parse(args[1]), Request = DbRequest.UseGenerated },
                    _ => throw new ArgumentException($"bad cmd argument, available arguments are: generate, useGenerated")
                };
                var builder = Host.CreateDefaultBuilder(args)
                    .ConfigureServices((hostContext, services) =>
                    {
                        Console.WriteLine($"db path is: {hostContext.Configuration.GetConnectionString("ExperimentDatabase")}");
                        
                        services.AddHostedService<DbExperimentWorker>();
                        services.AddSingleton<IDeckShuffler, RandomDeckShuffler>();
                        services.AddSingleton<ExperimentConditionService>();
                        services.AddDbContextFactory<ExperimentConditionContext>(
                            options => options.UseSqlite(
                                hostContext.Configuration.GetConnectionString("ExperimentDatabase")));
                        services.AddSingleton(myConfig);
                    });
                switch (args[2])
                {
                    case "http":
                        return builder.ConfigureServices((hostContext, services) =>
                        {
                            var uris = new List<Uri>
                            {
                                new(hostContext.Configuration.GetConnectionString("httpPlayer1")! + "/choose"),
                                new(hostContext.Configuration.GetConnectionString("httpPlayer2")! + "/choose")
                            };
                            var expConfig = new ExperimentConfig { Uris = uris };
                            services.AddSingleton(expConfig);
                            
                            services.AddScoped<IExperiment, NoShuffleHttpExperiment>();
                        });
                    
                    case "rabbitmq":
                        return builder.ConfigureServices((hostContext, services) =>
                            {
                                var uris = new List<Uri>
                                {
                                    new(hostContext.Configuration.GetConnectionString("httpPlayer1")! + "/color"),
                                    new(hostContext.Configuration.GetConnectionString("httpPlayer2")! + "/color"),
                                    new(hostContext.Configuration.GetConnectionString("mqPlayer1")!),
                                    new(hostContext.Configuration.GetConnectionString("mqPlayer2")!)
                                };
                                var expConfig = new ExperimentConfig {Uris = uris};
                                services.AddSingleton(expConfig);
                                
                                services.AddScoped<IExperiment, NoShuffleRabbitExperiment>();
                                services.AddMassTransit(x =>
                                {
                                    x.AddConsumer<CardIndexToldConsumer>();
                                    
                                    x.UsingRabbitMq((context, conf) =>
                                    {
                                        conf.Host(
                                            hostContext.Configuration["RabbitMQ:host"]!, 
                                            hostContext.Configuration["RabbitMQ:virtualHost"]!, 
                                            h =>
                                        {
                                            
                                            h.Username(hostContext.Configuration["RabbitMQ:user"]!);
                                            h.Password(hostContext.Configuration["RabbitMQ:password"]!);
                                        });
                                        
                                        conf.ReceiveEndpoint(hostContext.Configuration.GetConnectionString("publishUrl")!, e =>
                                        {
                                            e.ConfigureConsumer<CardIndexToldConsumer>(context);
                                        });
                                    });
                                });
                                var h = new CardIndexToldHandler();
                                services.AddSingleton<TellCardIndexProducer>();
                                services.AddSingleton<ICardIndexToldHandler>(h);
                                services.AddSingleton(h);
                            });
                    
                    default: throw new ArgumentException("3 argument must have value 'http' or 'rabbitmq'");
                }
                
            }
            default:
                throw new InvalidAmountOfArgumentsException($"wrong amount of arguments, expected 1 or 3 has {args.Length}");
        }
    }
}
