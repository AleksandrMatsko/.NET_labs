using CardLibrary;
using StrategyLibrary;

namespace dot_NET_labs;

public interface IExperiment
{
    bool Do();
}

public class SimpleExperiment : IExperiment
{
    private readonly ICardPickStrategy _maskCardPickStrategy;
    private readonly ICardPickStrategy _zuckerbergCardPickStrategy;
    private readonly IDeckShuffler _deckShuffler;
    private ShuffleableCardDeck? _cardDeck;

    public SimpleExperiment(IDeckShuffler deckShuffler, ICardPickStrategy maskCardPickStrategy, ICardPickStrategy zuckerbergCardPickStrategy)
    {
        _maskCardPickStrategy = maskCardPickStrategy;
        _zuckerbergCardPickStrategy = zuckerbergCardPickStrategy;
        _deckShuffler = deckShuffler;
    }
    
    public bool Do()
    {
        _cardDeck ??= Shuffleable36CardDeck.CreateCardDeck();
        
        _deckShuffler.Shuffle(ref _cardDeck);
        
        _cardDeck.Split(out var maskDeck, out var zuckerbergDeck);
        
        var maskChoice = _maskCardPickStrategy.Decide(maskDeck);
        var zuckerbergChoice = _zuckerbergCardPickStrategy.Decide(zuckerbergDeck);
        return maskDeck[zuckerbergChoice].Color == zuckerbergDeck[maskChoice].Color;
    }
}