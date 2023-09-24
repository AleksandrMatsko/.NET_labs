using CardLibrary;

namespace ColosseumTests;

public class CardDeckUtils
{
    public static Card[] PrepareCards(int count)
    {
        var cards = new Card[count];
        for (int i = 0; i < count; i++)
        {
            if (i % 2 == 0)
            {
                cards[i] = new Card(CardColor.Black, i);
            }
            else
            {
                cards[i] = new Card(CardColor.Red, i);
            }
        }

        return cards;
    }
}