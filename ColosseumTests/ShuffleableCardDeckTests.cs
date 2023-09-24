using CardLibrary;

namespace ColosseumTests;

[TestFixture]
public class ShuffleableCardDeckTests
{

    private static void Get2RandomIndexes(int maxVal, out int i, out int j)
    {
        var rnd = new Random();
        if (maxVal <= 1)
        {
            i = 0;
            j = 0;
            return;
        }
        i = rnd.Next(maxVal);
        j = rnd.Next(maxVal);
        while (i == j)
        {
            j = rnd.Next(maxVal);
        }
    }
    
    [Test]
    public void TestSwapInEmptyCardDeck()
    {
        var cards = CardDeckUtils.PrepareCards(0);
        var deck = new ShuffleableCardDeck(cards);
        
        Assert.DoesNotThrow(() => deck.SwapCards(10, 0));
    }

    [TestCase(2)]
    [TestCase(3)]
    [TestCase(10)]
    [TestCase(1_000)]
    public void TestSwapInCardDeck(int count)
    {
        var cards = CardDeckUtils.PrepareCards(count);
        var deck = new ShuffleableCardDeck(cards);

        Get2RandomIndexes(deck.Length, out var i, out var j);
        var iCard = deck[i];
        var jCard = deck[j];
        
        deck.SwapCards(i, j);
        
        Assert.That(iCard.Equals(deck[j]) && jCard.Equals(deck[i]), Is.True);
    }

    [Test]
    public void TestSplitInEmptyCardDeck()
    {
        var cards = CardDeckUtils.PrepareCards(0);
        var deck = new ShuffleableCardDeck(cards);
        
        deck.Split(out var firstDeck, out var secondDeck);
        
        Assert.That(firstDeck.Length.Equals(0) && secondDeck.Length.Equals(0), Is.True);
    }

    [TestCase(2)]
    [TestCase(4)]
    [TestCase(1_000)]
    public void TestSplitWithEvenCardsCount(int count)
    {
        var cards = CardDeckUtils.PrepareCards(count);
        var deck = new ShuffleableCardDeck(cards);
        
        deck.Split(out var firstDeck, out var secondDeck);

        if (firstDeck.Length != secondDeck.Length || firstDeck.Length != deck.Length / 2)
        {
            Assert.Fail();
        }

        for (var i = 0; i < firstDeck.Length; i++)
        {
            if (!firstDeck[i].Equals(deck[i]))
            {
                Assert.Fail();
            }

            if (!secondDeck[i].Equals(deck[firstDeck.Length + i]))
            {
                Assert.Fail();
            }
        }
        Assert.Pass();
    }

    [TestCase(1)]
    [TestCase(3)]
    [TestCase(1_001)]
    public void TestSplitWithNotEvenCardsCount(int count)
    {
        var cards = CardDeckUtils.PrepareCards(count);
        var deck = new ShuffleableCardDeck(cards);
        
        deck.Split(out var firstDeck, out var secondDeck);

        if (firstDeck.Length != deck.Length / 2 || secondDeck.Length != deck.Length / 2 + 1)
        {
            Assert.Fail();
        }

        for (var i = 0; i < firstDeck.Length; i++)
        {
            if (!firstDeck[i].Equals(deck[i]))
            {
                Assert.Fail();
            }
        }

        for (var i = 0; i < secondDeck.Length; i++)
        {
            if (!secondDeck[i].Equals(deck[firstDeck.Length + i]))
            {
                Assert.Fail();
            }
        }
        Assert.Pass();
    }
}