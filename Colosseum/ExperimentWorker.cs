using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Colosseum;

public class ExperimentWorker : BackgroundService
{
    public int ExperimentCount { get; set; } = 1_000_000;
    private readonly IExperiment _experiment;
    private readonly ILogger _logger;

    public ExperimentWorker(IExperiment experiment, ILogger<ExperimentWorker> logger)
    {
        _experiment = experiment;
        _logger = logger;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var success = 0;
        var experimentsCompleted = 0;
        for (int i = 0; i < ExperimentCount && !stoppingToken.IsCancellationRequested; i++)
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
        
        return Task.CompletedTask;
    }
}