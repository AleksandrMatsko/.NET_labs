using CardLibrary;

namespace StrategyLibrary;

public interface ICardPickStrategy
{
    int Choose(in CardDeck cardDeck);
}

public class PickFirstCardStrategy : ICardPickStrategy
{
    public int Choose(in CardDeck cardDeck)
    {
        return 0;
    }
}

public class RandomCardPickStrategy : ICardPickStrategy
{
    private static readonly Random rnd = new Random(); 
    
    public int Choose(in CardDeck cardDeck)
    {
        return rnd.Next(cardDeck.Length);
    }
}

public class PickLastCardPickStrategy : ICardPickStrategy
{
    public int Choose(in CardDeck cardDeck)
    {
        return cardDeck.Length - 1;
    }
}