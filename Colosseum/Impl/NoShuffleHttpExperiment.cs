using CardLibrary;
using Colosseum.Abstractions;
using Colosseum.Client;
using Colosseum.Exceptions;
using Microsoft.Extensions.Logging;

namespace Colosseum.Impl;

public class NoShuffleHttpExperiment : IExperiment
{
    private readonly ILogger<NoShuffleHttpExperiment> _logger;
    private readonly HttpPlayerAsker _firstAsker;
    private readonly HttpPlayerAsker _secondAsker;

    public NoShuffleHttpExperiment(
        ILogger<NoShuffleHttpExperiment> logger, 
        ExperimentConfig config)
    {
        _logger = logger;
        if (config.Uris.Count < 2)
        {
            throw new NotEnoughPlayersException($"expected 2 player's uris, have {config.Uris.Count}");
        }

        var lf = new LoggerFactory();
        _firstAsker = new HttpPlayerAsker(config.Uris[0], new Logger<HttpPlayerAsker>(lf));
        _secondAsker = new HttpPlayerAsker(config.Uris[1], new Logger<HttpPlayerAsker>(lf));
    }
    
    public bool Do(ShuffleableCardDeck cardDeck)
    {
        cardDeck.Split(out var firstDeck, out var secondDeck);

        var t1 = _firstAsker.Ask(firstDeck);
        var t2 = _secondAsker.Ask(secondDeck);

        Task.WaitAll(t1, t2);
        
        _logger.LogInformation($"Experiment participants: {t1.Result.Name} -> {t1.Result.CardNumber} and {t2.Result.Name} -> {t2.Result.CardNumber}");
        return firstDeck[t2.Result.CardNumber].Color == secondDeck[t1.Result.CardNumber].Color;
    }
}