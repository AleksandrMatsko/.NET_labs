using CardLibrary;
using CardLibrary.Abstractions;
using CardLibrary.Impl;
using CardStorage;
using Colosseum.Impl;
using Colosseum.Abstractions;
using Colosseum.Exceptions;
using Colosseum.Workers;
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
        if (args.Length == 0)
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
        if (args.Length == 1)
        {
            MyConfig myConfig;
            switch (args[0])
            {
                case "generate":
                    myConfig = new MyConfig { ExperimentCount = 100, Request = DbRequest.Generate };
                    break;
                case "useGenerated":
                    myConfig = new MyConfig { ExperimentCount = 100, Request = DbRequest.UseGenerated };
                    break;
                default:
                    throw new ArgumentException($"bad cmd argument, available arguments are: generate, useGenerated");
            }
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<DbExperimentWorker>();
                    services.AddSingleton<IDeckShuffler, RandomDeckShuffler>();
                    services.AddScoped<IExperiment, HttpExperiment>();
                    services.AddSingleton(new Uri("http://localhost:5031/player"));
                    services.AddSingleton(new Uri("http://localhost:5032/player"));
                    services.AddSingleton<ExperimentConditionService>();
                    services.AddSingleton<ExperimentConditionContext>();
                    services.AddSingleton(myConfig);
                });
        }

        throw new InvalidAmountOfArgumentsException($"wrong amount of arguments, expected 0 or 1 has {args.Length}");
    }
}