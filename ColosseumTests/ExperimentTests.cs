using CardLibrary;
using CardLibrary.Abstractions;
using Colosseum.Abstractions;
using Colosseum.Impl;
using Microsoft.Extensions.Logging;
using Moq;
using StrategyLibrary.Impl;

namespace ColosseumTests;

[TestFixture]
public class ExperimentTests
{
    
    private Mock<IDeckShuffler> _shufflerMock;
    private Mock<ShuffleableCardDeck> _deckMock;
    private Mock<AbstractPlayer> _elonMock;
    private Mock<AbstractPlayer> _markMock;
    private CardDeck _firstAfterSplit;
    private CardDeck _secondAfterSplit;
    
    
    [SetUp]
    public void SetUp()
    {
        _shufflerMock = new Mock<IDeckShuffler>();
        _shufflerMock.Setup(s => s.Shuffle(It.IsAny<ShuffleableCardDeck>()));

        var deck = Shuffleable36CardDeck.CreateCardDeck();
        var cardList = new List<Card>();
        for (var i = 0; i < deck.Length; i++)
        {
            cardList.Add(deck[i]);
        }
        
        deck.Split(out _firstAfterSplit, out _secondAfterSplit);
        _deckMock = new Mock<ShuffleableCardDeck>((IList<Card>)cardList);
        _deckMock.Setup(d => d.Split(out _firstAfterSplit, out _secondAfterSplit));

        _elonMock = new Mock<AbstractPlayer>(new PickFirstCardStrategy());
        _elonMock.Setup(e => e.Choose(_firstAfterSplit)).Returns(0);
        _elonMock.Setup(e => e.Name).Returns("Elon Mask");

        _markMock = new Mock<AbstractPlayer>(new PickFirstCardStrategy());
        _markMock.Setup(m => m.Choose(_secondAfterSplit)).Returns(0);
        _markMock.Setup(m => m.Name).Returns("Mark Zuckerberg");
    }
    
    [Test]
    public void TestSimpleExperiment()
    {
        var lf = new LoggerFactory();
        var experiment = new SimpleExperiment(
            _deckMock.Object, 
            _shufflerMock.Object,
            lf.CreateLogger<SimpleExperiment>(),
            new[]{_elonMock.Object, _markMock.Object});
        var result = experiment.Do();
        
        Assert.DoesNotThrow(() =>
        {
            _shufflerMock.Verify(s => s.Shuffle(It.IsAny<ShuffleableCardDeck>()), Times.AtLeastOnce);
            _deckMock.Verify(deck => deck.Split(out _firstAfterSplit, out _secondAfterSplit), Times.Once);
            _elonMock.Verify(e => e.Choose(It.IsAny<CardDeck>()), Times.Once);
            _markMock.Verify(e => e.Choose(It.IsAny<CardDeck>()), Times.Once);
        });
        
        Assert.That(result, Is.EqualTo(_firstAfterSplit[0].Color == _secondAfterSplit[0].Color));
    }
}