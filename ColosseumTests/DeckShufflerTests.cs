using CardLibrary;
using CardLibrary.Impl;
using ColosseumTests.DeckTests;

namespace ColosseumTests;

[TestFixture]
public class RandomDeckShufflerTests
{
    // The probability that the deck will not be shuffled by a RandomDeckShuffler is considered very low
    [TestCase(10)]
    [TestCase(36)]
    public void Shuffle_ShuffleableCardDeck_SwapsAtLeast2Cards(int count)
    {
        var cards = CardDeckUtils.PrepareCards(count);
        var deckCards = new Card[cards.Length];
        cards.CopyTo(deckCards, 0);
        var deck = new ShuffleableCardDeck(deckCards);

        var shuffler = new RandomDeckShuffler();
        
        shuffler.Shuffle(deck);

        for (var i = 0; i < deck.Length; i++)
        {
            if (!deck[i].Equals(cards[i]))
            {
                Assert.Pass();
            }
        }
        
        Assert.Fail();
    }
}