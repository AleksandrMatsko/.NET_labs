using CardLibrary;
using StrategyLibrary;

namespace dot_NET_labs;

public interface IExperiment
{
    bool Do();
}

public class SimpleExperiment : IExperiment
{
    private readonly IStrategy _maskStrategy;
    private readonly IStrategy _zuckerbergStrategy;
    private readonly IDeckShuffler _deckShuffler;
    private ShuffleableCardDeck? _cardDeck;

    public SimpleExperiment(IDeckShuffler deckShuffler, IStrategy maskStrategy, IStrategy zuckerbergStrategy)
    {
        _maskStrategy = maskStrategy;
        _zuckerbergStrategy = zuckerbergStrategy;
        _deckShuffler = deckShuffler;
    }
    
    public bool Do()
    {
        _cardDeck ??= Shuffleable36CardDeck.CreateCardDeck();
        
        _deckShuffler.Shuffle(ref _cardDeck);
        
        _cardDeck.Split(out var maskDeck, out var zuckerbergDeck);
        
        var maskChoice = _maskStrategy.Decide(maskDeck);
        var zuckerbergChoice = _zuckerbergStrategy.Decide(zuckerbergDeck);
        return maskDeck[zuckerbergChoice].Color == zuckerbergDeck[maskChoice].Color;
    }
}