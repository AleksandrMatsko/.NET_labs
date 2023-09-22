using CardLibrary;
using StrategyLibrary.Interfaces;

namespace StrategyLibrary.Impl;

public class PickFirstCardStrategy : ICardPickStrategy
{
    public int Choose(in CardDeck cardDeck)
    {
        return 0;
    }
}

public class RandomCardPickStrategy : ICardPickStrategy
{
    private static readonly Random Rnd = new Random(); 
    
    public int Choose(in CardDeck cardDeck)
    {
        return Rnd.Next(cardDeck.Length);
    }
}

public class PickLastCardStrategy : ICardPickStrategy
{
    public int Choose(in CardDeck cardDeck)
    {
        return cardDeck.Length - 1;
    }
}