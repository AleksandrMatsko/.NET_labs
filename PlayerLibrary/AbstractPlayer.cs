using CardLibrary;
using StrategyLibrary.Interfaces;

namespace PlayerLibrary;

public abstract class AbstractPlayer
{
    public abstract string Name { get; }
    private readonly ICardPickStrategy _strategy;

    protected AbstractPlayer(ICardPickStrategy strategy)
    {
        _strategy = strategy;
    }

    public virtual int Choose(CardDeck cardDeck)
    {
        return _strategy.Choose(cardDeck);
    }
}
