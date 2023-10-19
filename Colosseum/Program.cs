using CardLibrary.Abstractions;
using CardLibrary.Impl;
using CardStorage;
using Colosseum.Impl;
using Colosseum.Abstractions;
using Colosseum.Exceptions;
using Colosseum.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            case 0:
            {
                var myConfig = new MyConfig {ExperimentCount = 1_000_000, Request = DbRequest.None};
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
            case 1:
            {
                var myConfig = args[0] switch
                {
                    "generate" => new MyConfig { ExperimentCount = 100, Request = DbRequest.Generate },
                    "useGenerated" => new MyConfig { ExperimentCount = 100, Request = DbRequest.UseGenerated },
                    _ => throw new ArgumentException($"bad cmd argument, available arguments are: generate, useGenerated")
                };
                var dbPath = Path.Join(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "cards.db");
                Console.WriteLine($"db path is: {dbPath}");
                return Host.CreateDefaultBuilder(args)
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<DbExperimentWorker>();
                        services.AddSingleton<IDeckShuffler, RandomDeckShuffler>();
                        services.AddScoped<IExperiment, NoShuffleHttpExperiment>();
                        services.AddSingleton(new Uri("http://localhost:5031/player"));
                        services.AddSingleton(new Uri("http://localhost:5032/player"));
                        services.AddSingleton<ExperimentConditionService>();
                        services.AddDbContextFactory<ExperimentConditionContext>(
                            options => options.UseSqlite($"Data Source={dbPath}"));
                        services.AddSingleton(myConfig);
                    });
            }
            default:
                throw new InvalidAmountOfArgumentsException($"wrong amount of arguments, expected 0 or 1 has {args.Length}");
        }
    }
}