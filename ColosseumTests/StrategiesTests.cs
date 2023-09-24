using CardLibrary;
using ColosseumTests.DeckTests;
using StrategyLibrary.Impl;

namespace ColosseumTests;

[TestFixture]
public class StrategiesTests
{
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(1_000)]
    public void TestPickFirstStrategy(int count)
    {
        var deck = new CardDeck(CardDeckUtils.PrepareCards(count));
        var strategy = new PickFirstCardStrategy();
        
        Assert.That(strategy.Choose(deck), Is.EqualTo(0));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(1_000)]
    public void TestPickLastStrategy(int count)
    {
        var deck = new CardDeck(CardDeckUtils.PrepareCards(count));
        var strategy = new PickLastCardStrategy();
        
        Assert.That(strategy.Choose(deck), Is.EqualTo(deck.Length - 1));
    }
}