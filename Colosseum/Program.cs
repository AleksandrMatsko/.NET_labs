using CardLibrary.Abstractions;
using CardLibrary.Impl;
using CardStorage;
using Colosseum.Impl;
using Colosseum.Abstractions;
using Colosseum.Exceptions;
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
                switch (args[2])
                {
                    case "http":
                        return Host.CreateDefaultBuilder(args)
                            .ConfigureServices((hostContext, services) =>
                            {
                                Console.WriteLine($"db path is: {hostContext.Configuration.GetConnectionString("ExperimentDatabase")}");
                                var uris = new List<Uri>();
                                uris.Add(new Uri(hostContext.Configuration.GetConnectionString("Player1")!+"/choose"));
                                uris.Add(new Uri(hostContext.Configuration.GetConnectionString("Player2")!+"/choose"));
                                var expConfig = new ExperimentConfig {Uris = uris};
                                services.AddHostedService<DbExperimentWorker>();
                                services.AddSingleton<IDeckShuffler, RandomDeckShuffler>();
                                services.AddScoped<IExperiment, NoShuffleHttpExperiment>();
                                services.AddSingleton<ExperimentConditionService>();
                                services.AddDbContextFactory<ExperimentConditionContext>(
                                    options => options.UseSqlite(
                                        hostContext.Configuration.GetConnectionString("ExperimentDatabase")));
                                services.AddSingleton(myConfig);
                                services.AddSingleton(expConfig);
                            });
                    
                    case "rabbitmq":
                        return Host.CreateDefaultBuilder(args)
                            .ConfigureServices((hostContext, services) =>
                            {
                                Console.WriteLine($"db path is: {hostContext.Configuration.GetConnectionString("ExperimentDatabase")}");
                                var uris = new List<Uri>();
                                uris.Add(new Uri(hostContext.Configuration.GetConnectionString("Player1")!+"/color"));
                                uris.Add(new Uri(hostContext.Configuration.GetConnectionString("Player2")!+"/color"));
                                var expConfig = new ExperimentConfig {Uris = uris};
                                services.AddHostedService<DbExperimentWorker>();
                                services.AddSingleton<IDeckShuffler, RandomDeckShuffler>();
                                services.AddScoped<IExperiment, NoShuffleRabbitExperiment>();
                                services.AddSingleton<ExperimentConditionService>();
                                services.AddDbContextFactory<ExperimentConditionContext>(
                                    options => options.UseSqlite(
                                        hostContext.Configuration.GetConnectionString("ExperimentDatabase")));
                                services.AddSingleton(myConfig);
                                services.AddSingleton(expConfig);
                                services.AddMassTransit(x =>
                                {
                                    x.UsingRabbitMq((context, conf) =>
                                    {
                                        conf.Host("localhost", "/", h =>
                                        {
                                            h.Username("rmuser");
                                            h.Password("rmpassword");
                                        });
                                    });
                                });
                                services.AddScoped<TellCardIndexProducer>();
                            });
                    
                    default: throw new ArgumentException("3 argument must have value http or rabbitmq");
                }
                
            }
            default:
                throw new InvalidAmountOfArgumentsException($"wrong amount of arguments, expected 1 or 3 has {args.Length}");
        }
    }
}
