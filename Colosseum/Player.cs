using CardLibrary;
using StrategyLibrary;

namespace Colosseum;

public abstract class Player
{
    public abstract string Name { get; }
    private readonly ICardPickStrategy _strategy;

    protected Player(ICardPickStrategy strategy)
    {
        _strategy = strategy;
    }

    public int Choose(CardDeck cardDeck)
    {
        return _strategy.Choose(cardDeck);
    }
}

public class ElonMask : Player
{
    public ElonMask(ICardPickStrategy strategy) : base(strategy)
    {
    }

    public override string Name => "Elon Mask";
}

public class MarkZuckerberg : Player
{
    public MarkZuckerberg(ICardPickStrategy strategy) : base(strategy)
    {
    }

    public override string Name => "Mark Zuckerberg";
}