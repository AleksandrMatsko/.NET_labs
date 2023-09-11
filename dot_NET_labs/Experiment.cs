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
    private ShuffledCardDeck? _cardDeck;

    public SimpleExperiment(IStrategy maskStrategy, IStrategy zuckerbergStrategy)
    {
        _maskStrategy = maskStrategy;
        _zuckerbergStrategy = zuckerbergStrategy;
    }
    
    public bool Do()
    {
        _cardDeck ??= Shuffled36CardDeck.CreateCardDeck();
        
        _cardDeck.Shuffle();
        _cardDeck.Split(out var maskDeck, out var zuckerbergDeck);
        
        var maskChoice = _maskStrategy.Decide(maskDeck);
        var zuckerbergChoice = _zuckerbergStrategy.Decide(zuckerbergDeck);
        return maskDeck[zuckerbergChoice].Color == zuckerbergDeck[maskChoice].Color;
    }
}