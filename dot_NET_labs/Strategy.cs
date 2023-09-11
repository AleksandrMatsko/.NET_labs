namespace dot_NET_labs;

public interface IStrategy
{
    int Decide(in CardDeck cardDeck);
}

public class PickFirstStrategy : IStrategy
{
    public int Decide(in CardDeck cardDeck)
    {
        return 0;
    }
}

public class RandomStrategy : IStrategy
{
    private static readonly Random rnd = new Random(); 
    
    public int Decide(in CardDeck cardDeck)
    {
        return rnd.Next(cardDeck.Length);
    }
}

public class PickLastStrategy : IStrategy
{
    public int Decide(in CardDeck cardDeck)
    {
        return cardDeck.Length - 1;
    }
}