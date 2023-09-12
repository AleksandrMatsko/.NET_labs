using CardLibrary;

namespace StrategyLibrary;

public interface ICardPickStrategy
{
    int Decide(in CardDeck cardDeck);
}

public class PickFirstCardPickStrategy : ICardPickStrategy
{
    public int Decide(in CardDeck cardDeck)
    {
        return 0;
    }
}

public class RandomCardPickStrategy : ICardPickStrategy
{
    private static readonly Random rnd = new Random(); 
    
    public int Decide(in CardDeck cardDeck)
    {
        return rnd.Next(cardDeck.Length);
    }
}

public class PickLastCardPickStrategy : ICardPickStrategy
{
    public int Decide(in CardDeck cardDeck)
    {
        return cardDeck.Length - 1;
    }
}