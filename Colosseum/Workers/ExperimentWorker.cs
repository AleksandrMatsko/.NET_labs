using CardLibrary;
using Colosseum.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Colosseum.Workers;

public class ExperimentWorker : BackgroundService
{
    public int ExperimentCount { get; }
    private readonly IExperiment _experiment;
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ShuffleableCardDeck _cardDeck;

    public ExperimentWorker(
        IExperiment experiment, 
        ILogger<ExperimentWorker> logger, 
        IHostApplicationLifetime lifetime, 
        MyConfig config)
    {
        _experiment = experiment;
        _logger = logger;
        _lifetime = lifetime;
        _cardDeck = Shuffleable36CardDeck.CreateCardDeck();
        ExperimentCount = config.ExperimentCount;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var success = 0;
        var experimentsCompleted = 0;
        int i = 0;
        try
        {
            for (i = 0; i < ExperimentCount && !stoppingToken.IsCancellationRequested; i++)
            {
                if (_experiment.Do(_cardDeck))
                {
                    success += 1;
                }

                experimentsCompleted += 1;
                
                if (i % 100000 == 0)
                {
                    _logger.LogInformation($"Completed {i} iteration");
                }
            }

            Console.WriteLine($"\nExperiments completed: {experimentsCompleted}");
            Console.WriteLine($"Success rate: {(double)success / experimentsCompleted}\n");
        }
        catch (Exception e)
        {
            _logger.LogCritical($"iteration {i}: {e.Message}");

        }
        finally
        {
            _lifetime.StopApplication();
        }
        
        return Task.CompletedTask;
    }
}