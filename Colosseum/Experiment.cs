using CardLibrary;
using Microsoft.Extensions.Logging;

namespace Colosseum;

public interface IExperiment
{
    bool Do();
}

public class SimpleExperiment : IExperiment
{
    private readonly Player _firstPlayer;
    private readonly Player _secondPlayer;
    private readonly IDeckShuffler _deckShuffler;
    private ShuffleableCardDeck? _cardDeck;

    public SimpleExperiment(
        IDeckShuffler deckShuffler, 
        Player firstPlayer, 
        Player secondPlayer, 
        ILogger<SimpleExperiment> logger)
    {
        _firstPlayer = firstPlayer;
        _secondPlayer = secondPlayer;
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