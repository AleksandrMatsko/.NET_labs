namespace StrategyLibrary;

public interface IDeckShuffler
{
    void Shuffle(ref CardDeck cardDeck);
}

public class RandomDeckShuffler : IDeckShuffler
{
    private static readonly Random Rnd = new Random();
    
    public void Shuffle(ref CardDeck cardDeck)
    {
        for (var i = cardDeck.Length - 1; i >= 0; i--)
        {
            var j = Rnd.Next(i + 1);
            cardDeck.SwapCards(i, j);
        }
    }
}