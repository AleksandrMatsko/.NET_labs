using CardLibrary;
using CardLibrary.Abstractions;
using Colosseum.Abstractions;
using Colosseum.Exceptions;
using Microsoft.Extensions.Logging;
using PlayerLibrary;

namespace Colosseum.Experiments;

public class SimpleExperiment : IExperiment
{
    private readonly AbstractPlayer _firstPlayer;
    private readonly AbstractPlayer _secondPlayer;
    private readonly IDeckShuffler _deckShuffler;

    public SimpleExperiment(
        IDeckShuffler deckShuffler, 
        ILogger<SimpleExperiment> logger, 
        IEnumerable<AbstractPlayer> players)
    {
        var enumerable = players as AbstractPlayer[] ?? players.ToArray();
        if (enumerable.Length < 2)
        {
            throw new NotEnoughPlayersException($"expected 2 players, have {enumerable.Length}");
        }
        _firstPlayer = enumerable[0];
        _secondPlayer = enumerable[1];
        _deckShuffler = deckShuffler;

        logger.LogInformation($"Experiment participants: {_firstPlayer.Name} and {_secondPlayer.Name}");
    }
    
    public bool Do(ShuffleableCardDeck cardDeck)
    {
        _deckShuffler.Shuffle(cardDeck);
        
        cardDeck.Split(out var firstDeck, out var secondDeck);
        
        var firstChoice = _firstPlayer.Choose(firstDeck);
        var secondChoice = _secondPlayer.Choose(secondDeck);
        return firstDeck[secondChoice].Color == secondDeck[firstChoice].Color;
    }
}

