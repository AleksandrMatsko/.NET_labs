using CardLibrary;
using CardLibrary.Abstractions;
using Colosseum.Abstractions;
using Microsoft.Extensions.Logging;

namespace Colosseum;

public class SimpleExperiment : IExperiment
{
    private readonly AbstractPlayer _firstPlayer;
    private readonly AbstractPlayer _secondPlayer;
    private readonly IDeckShuffler _deckShuffler;
    private ShuffleableCardDeck? _cardDeck;

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
    
    public bool Do()
    {
        _cardDeck ??= Shuffleable36CardDeck.CreateCardDeck();
        
        _deckShuffler.Shuffle(_cardDeck);
        
        _cardDeck.Split(out var firstDeck, out var secondDeck);
        
        var firstChoice = _firstPlayer.Choose(firstDeck);
        var secondChoice = _secondPlayer.Choose(secondDeck);
        return firstDeck[secondChoice].Color == secondDeck[firstChoice].Color;
    }
}