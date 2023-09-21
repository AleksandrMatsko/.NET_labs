using CardLibrary;
using StrategyLibrary;

namespace Colosseum;

public class Player
{
    public string Name { get; }
    private ICardPickStrategy _strategy;

    public Player(string name, ICardPickStrategy strategy)
    {
        Name = name;
        _strategy = strategy;
    }

    public int Choose(CardDeck cardDeck)
    {
        return _strategy.Choose(cardDeck);
    }
}