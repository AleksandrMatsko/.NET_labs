namespace CardLibrary;

public interface IDeckShuffler
{
    void Shuffle(ShuffleableCardDeck cardDeck);
}

public class RandomDeckShuffler : IDeckShuffler
{
    private static readonly Random Rnd = new();
    
    // uses Fisher–Yates shuffle
    public void Shuffle(ShuffleableCardDeck cardDeck)
    {
        for (var i = cardDeck.Length - 1; i >= 0; i--)
        {
            var j = Rnd.Next(i + 1);
            cardDeck.SwapCards(i, j);
        }
    }
}