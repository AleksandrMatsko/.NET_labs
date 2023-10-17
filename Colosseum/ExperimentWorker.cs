using Colosseum.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Colosseum;

public class ExperimentWorker : BackgroundService
{
    public int ExperimentCount { get; set; } = 100;
    private readonly IExperiment _experiment;
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _lifetime;

    public ExperimentWorker(IExperiment experiment, ILogger<ExperimentWorker> logger, IHostApplicationLifetime lifetime)
    {
        _experiment = experiment;
        _logger = logger;
        _lifetime = lifetime;
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
                if (i % 100000 == 0)
                {
                    _logger.LogInformation($"Completed {i} iteration");
                }

                if (_experiment.Do())
                {
                    success += 1;
                }

                experimentsCompleted += 1;
            }

            Console.WriteLine($"Experiments completed: {experimentsCompleted}");
            Console.WriteLine($"Success rate: {(double)success / (experimentsCompleted)}");
        }
        catch (Exception e)
        {
            _logger.LogCritical(e.Message);

        }
        finally
        {
            _lifetime.StopApplication();
        }
        
        return Task.CompletedTask;
    }
}