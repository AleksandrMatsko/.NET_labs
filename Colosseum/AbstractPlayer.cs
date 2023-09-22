using CardLibrary;
using StrategyLibrary;

namespace Colosseum;

public abstract class AbstractPlayer
{
    public abstract string Name { get; }
    private readonly ICardPickStrategy _strategy;

    protected AbstractPlayer(ICardPickStrategy strategy)
    {
        _strategy = strategy;
    }

    public int Choose(CardDeck cardDeck)
    {
        return _strategy.Choose(cardDeck);
    }
}

public class ElonMask : AbstractPlayer
{
    public ElonMask(ICardPickStrategy strategy) : base(strategy)
    {
    }

    public override string Name => "Elon Mask";
}

public class MarkZuckerberg : AbstractPlayer
{
    public MarkZuckerberg(ICardPickStrategy strategy) : base(strategy)
    {
    }

    public override string Name => "Mark Zuckerberg";
}