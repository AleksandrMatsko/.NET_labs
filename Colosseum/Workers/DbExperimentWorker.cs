using CardLibrary;
using CardLibrary.Abstractions;
using CardStorage;
using Colosseum.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Colosseum.Workers;

public class DbExperimentWorker : BackgroundService
{
    private readonly ILogger<DbExperimentWorker> _logger;
    private readonly ExperimentConditionService _service;
    private readonly MyConfig _config;
    private readonly IExperiment _experiment;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IDeckShuffler _shuffler;

    public DbExperimentWorker(
        ILogger<DbExperimentWorker> logger, 
        ExperimentConditionService service, 
        MyConfig config, 
        IExperiment experiment, 
        IHostApplicationLifetime lifetime,
        IDeckShuffler shuffler)
    {
        _logger = logger;
        _service = service;
        _config = config;
        _experiment = experiment;
        _lifetime = lifetime;
        _shuffler = shuffler;
    }

    private Task Generate(CancellationToken stoppingToken)
    {
        try
        {
            _service.RecreateDb();
            for (var i = 0; i < _config.ExperimentCount && !stoppingToken.IsCancellationRequested; i++)
            {
                var deck = Shuffleable36CardDeck.CreateCardDeck();
                _shuffler.Shuffle(deck);
                _service.AddOne(deck);
            }
            _logger.LogInformation($"generated {_config.ExperimentCount} experiments");
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

    private Task UseGenerated(CancellationToken stoppingToken)
    {
        var success = 0;
        var experimentsCompleted = 0;
        try
        {
            var decks = _service.GetFirstN(_config.ExperimentCount);
            for (var i = 0; i < decks.Count && !stoppingToken.IsCancellationRequested; i++)
            {
                var cardsList = decks[i].Cast<Card>().ToList();

                if (_experiment.Do(new ShuffleableCardDeck(cardsList)))
                {
                    success += 1;
                }

                experimentsCompleted += 1;
                _logger.LogInformation($"experiments completed = {experimentsCompleted}");
            }
            
            Console.WriteLine($"\nExperiments completed: {experimentsCompleted}");
            Console.WriteLine($"Success rate: {(double)success / experimentsCompleted}\n");
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
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        switch (_config.Request)
        {
            case DbRequest.Generate:
            {
                _logger.LogInformation("start generating experiment conditions");
                return Generate(stoppingToken);
            }
            case DbRequest.UseGenerated:
            {
                _logger.LogInformation("using already generated experiment conditions");
                return UseGenerated(stoppingToken);
            }
            case DbRequest.None:
            {
                _logger.LogInformation("no request provided, stopping application");
                _lifetime.StopApplication();
                return Task.CompletedTask;
            }
            default:
            {
                _logger.LogError("bad DbRequest in config provided, stopping application");
                _lifetime.StopApplication();
                return Task.CompletedTask;
            }
        }
    }
}
