using CardLibrary;

namespace ColosseumTests.DeckTests;

[TestFixture]
public class Shuffleable36CardDeckTests
{
    [Test]
    public void Shuffleable36CardDeck_Has_36Cards()
    {
        var deck = Shuffleable36CardDeck.CreateCardDeck();
        
        Assert.That(deck.Length, Is.EqualTo(36));
    }

    [Test]
    public void Shuffleable36CardDeck_Has_EqualAmountOfBlackAndRedCards()
    {
        var deck = Shuffleable36CardDeck.CreateCardDeck();

        var blackCount = 0;
        var redCount = 0;
        for (var i = 0; i < deck.Length; i++)
        {
            if (deck[i].Color == CardColor.Black)
            {
                blackCount += 1;
            }
            else if (deck[i].Color == CardColor.Red)
            {
                redCount += 1;
            }
        }
        
        Assert.That(blackCount, Is.EqualTo(redCount));
    }
}