using CardLibrary;

namespace ColosseumTests;

[TestFixture]
public class CardDeckTests
{
    private Card[] PrepareCards(int count)
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
    
    
    [SetUp]
    public void Setup()
    {
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(1_000)]
    public void TestCardDeckLength(int count)
    {
        var cards = PrepareCards(count);
        var deck = new CardDeck(cards);

        Assert.That(cards, Has.Length.EqualTo(deck.Length));
    }

    [Test]
    public void TestIndexOperatorWithEmptyCardDeck()
    {
        var cards = PrepareCards(0);
        var deck = new CardDeck(cards);

        Assert.Throws<IndexOutOfRangeException>(() =>
        {
            var card = deck[0];
        });
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(10)]
    [TestCase(1_000)]
    public void TestIndexOperatorWithNonEmptyCardDeck(int count)
    {
        var cards = PrepareCards(count);
        var deck = new CardDeck(cards);
        
        for (var i = 0; i < cards.Length; i++)
        {
            if (!cards[i].Equals(deck[i]))
            {
                Assert.Fail("Cards do not match");
            }
        }
        
        Assert.Pass();
    }
}