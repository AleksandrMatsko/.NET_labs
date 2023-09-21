using CardLibrary;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StrategyLibrary;

namespace Colosseum;

class Program
{
    public static void Main(string[] args)
    {
       CreateHostBuilder(args).Build().RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<ExperimentWorker>();
                services.AddSingleton<IDeckShuffler, RandomDeckShuffler>();
                services.AddScoped<IExperiment, SimpleExperiment>();
                services.AddSingleton<Player>(new ElonMask(new PickFirstCardStrategy()));
                services.AddSingleton<Player>(new MarkZuckerberg(new PickFirstCardStrategy()));
            });
    }
}