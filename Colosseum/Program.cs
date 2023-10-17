using CardLibrary;
using CardLibrary.Abstractions;
using CardLibrary.Impl;
using Colosseum.Impl;
using Colosseum.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Colosseum;

class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<ExperimentWorker>();
                services.AddSingleton<IDeckShuffler, RandomDeckShuffler>();
                //services.AddScoped<IExperiment, SimpleExperiment>();
                //services.AddSingleton<AbstractPlayer>(new ElonMask(new PickFirstCardStrategy()));
                //services.AddSingleton<AbstractPlayer>(new MarkZuckerberg(new PickFirstCardStrategy()));
                services.AddScoped<IExperiment, HttpExperiment>();
                services.AddSingleton(new Uri("http://localhost:5031/player"));
                services.AddSingleton(new Uri("http://localhost:5031/player"));
                services.AddSingleton<ShuffleableCardDeck>(Shuffleable36CardDeck.CreateCardDeck());
            });
    }
}