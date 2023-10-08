using CardLibrary;
using Colosseum.Impl;
using ColosseumTests.DeckTests;
using Moq;
using StrategyLibrary.Interfaces;

namespace ColosseumTests;

[TestFixture]
public class PlayersTest
{
    private static readonly Random Rnd = new();
    private CardDeck _deck;
    private int _retVal;
    private Mock<ICardPickStrategy> _strategyMock;

    [SetUp]
    public void SetUp()
    {
        var deckSize = Rnd.Next(1000);
        _deck = new CardDeck(CardDeckUtils.PrepareCards(deckSize));
        _retVal = Rnd.Next(deckSize);
        _strategyMock = new Mock<ICardPickStrategy>();
        _strategyMock.Setup(s => s.Choose(_deck)).Returns(_retVal);
    }
        
    [Test]
    public void ElonMask_CallsStrategyMethodChoose_OnlyOnceAndHasSameRetValsWithPassedStrategy()
    {
        var player = new ElonMask(_strategyMock.Object);
        
        var playerChoice = player.Choose(_deck);
            
        Assert.DoesNotThrow(() =>
        {
            _strategyMock.Verify(s => s.Choose(_deck), Times.Once);
        });
        Assert.That(playerChoice, Is.EqualTo(_retVal));
    }
    
    [Test]
    public void MarkZuckerberg_CallsStrategyMethodChoose_OnlyOnceAndHasSameRetValsWithPassedStrategy()
    {
        var player = new MarkZuckerberg(_strategyMock.Object);
        
        var playerChoice = player.Choose(_deck);
            
        Assert.DoesNotThrow(() =>
        {
            _strategyMock.Verify(s => s.Choose(_deck), Times.Once);
        });
        Assert.That(playerChoice, Is.EqualTo(_retVal));
    }
}