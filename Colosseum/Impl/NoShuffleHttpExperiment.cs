using CardLibrary;
using Colosseum.Abstractions;
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
        IEnumerable<Uri> uris)
    {
        _logger = logger;
        var urisArr = uris as Uri[] ?? uris.ToArray();
        if (urisArr.Length < 2)
        {
            throw new NotEnoughPlayersException($"expected 2 players, have {urisArr.Length}");
        }

        var lf = new LoggerFactory();
        _firstAsker = new HttpPlayerAsker(urisArr[0], new Logger<HttpPlayerAsker>(lf));
        _secondAsker = new HttpPlayerAsker(urisArr[1], new Logger<HttpPlayerAsker>(lf));
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