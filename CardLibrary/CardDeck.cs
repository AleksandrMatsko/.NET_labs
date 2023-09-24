namespace CardLibrary;

public class CardDeck
{
    protected readonly Card[] Cards;
    
    public Card this[int i] => Cards[i];
    
    public int Length => Cards.Length;

    public CardDeck(Card[] cards)
    {
        Cards = cards;
    }
}

public class ShuffleableCardDeck : CardDeck
{
    public ShuffleableCardDeck(Card[] cards) : base(cards)
    {
    }
    
    // swaps i and j cards
    public void SwapCards(int i, int j)
    {
        if (i >= Cards.Length || j >= Cards.Length) return;
        (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
    }

    // splits card deck into two card decks
    // first == first half of the deck
    // second == second half of the deck
    // if CardsDeck.Length is not even first will have CardDeck.Length / 2 cards
    public void Split(out CardDeck first, out CardDeck second)
    {
        var mid = Cards.Length / 2;
        first = new CardDeck(Cards.Take(mid).ToArray());
        second = new CardDeck(Cards.Skip(mid).ToArray());
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