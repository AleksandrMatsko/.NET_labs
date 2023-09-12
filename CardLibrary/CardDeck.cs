namespace CardLibrary;

public class CardDeck
{
    protected Card[] Cards;
    
    public Card this[int i] => Cards[i];
    
    public int Length => Cards.Length;

    public CardDeck(Card[] cards)
    {
        Cards = cards;
    }

    public override string ToString()
    {
        string line = "";
        for (int i = 0; i < Cards.Length; i++)
        {
            line += $"{Cards[i].Number} ";
        }
        return line;
    }
}

public class ShuffleableCardDeck : CardDeck
{
    protected ShuffleableCardDeck(Card[] cards) : base(cards)
    {
    }
    
    // swaps i and j cards
    public void SwapCards(int i, int j)
    {
        (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
    }

    // splits card deck into two card decks
    // first == first half of the deck
    // second == second half of the deck
    // if CardsDeck.Length is not even should throw an exception
    public void Split(out CardDeck first, out CardDeck second)
    {
        var len = Cards.Length;
        if (len % 2 != 0)
        {
            // TODO: throw exception
        }

        var mid = len / 2;
        first = new CardDeck(Cards.Take(len / 2).ToArray());
        second = new CardDeck(Cards.Skip(len / 2).ToArray());
    }
}

public class Shuffleable36CardDeck : ShuffleableCardDeck
{
    private Shuffleable36CardDeck(Card[] cards) : base(cards)
    {
    }

    public static Shuffleable36CardDeck CreateCardDeck()
    {
        Card[] cards = new Card[36];
        for (int i = 0; i < 36; i += 2)
        {
            cards[i] = new Card(CardColor.Black, i);
            cards[i + 1] = new Card(CardColor.Red, i + 1);
        }
        return new Shuffleable36CardDeck(cards);
    }
}