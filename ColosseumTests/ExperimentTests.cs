﻿using CardLibrary;
using CardLibrary.Abstractions;
using Colosseum.Experiments;
using Microsoft.Extensions.Logging;
using Moq;
using PlayerLibrary;
using StrategyLibrary.Impl;

namespace ColosseumTests;

[TestFixture]
public class SimpleExperimentTests
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
        _deckMock = new Mock<ShuffleableCardDeck>(cardList);
        _deckMock.Setup(d => d.Split(out _firstAfterSplit, out _secondAfterSplit));

        _elonMock = new Mock<AbstractPlayer>(new PickFirstCardStrategy());
        _elonMock.Setup(e => e.Choose(_firstAfterSplit)).Returns(0);
        _elonMock.Setup(e => e.Name).Returns("Elon Mask");

        _markMock = new Mock<AbstractPlayer>(new PickFirstCardStrategy());
        _markMock.Setup(m => m.Choose(_secondAfterSplit)).Returns(0);
        _markMock.Setup(m => m.Name).Returns("Mark Zuckerberg");
    }
    
    [Test]
    public void SimpleExperiment_CallsShuffle_OnlyOnce()
    {
        var lf = new LoggerFactory();
        var experiment = new SimpleExperiment(
            _shufflerMock.Object,
            lf.CreateLogger<SimpleExperiment>(),
            new[]{_elonMock.Object, _markMock.Object});
        
        var result = experiment.Do(_deckMock.Object);
        
        Assert.DoesNotThrow(() =>
        {
            _shufflerMock.Verify(s => s.Shuffle(It.IsAny<ShuffleableCardDeck>()), Times.Once);
        });
    }

    [Test]
    public void SimpleExperiment_CallsSplit_OnlyOnce()
    {
        var lf = new LoggerFactory();
        var experiment = new SimpleExperiment(
            _shufflerMock.Object,
            lf.CreateLogger<SimpleExperiment>(),
            new[] { _elonMock.Object, _markMock.Object });

        var result = experiment.Do(_deckMock.Object);

        Assert.DoesNotThrow(() =>
        {
            _deckMock.Verify(deck => deck.Split(out _firstAfterSplit, out _secondAfterSplit), Times.Once);
        });
    }

    [Test]
    public void SimpleExperiment_CallsChooseForEachPlayer_OnlyOnce()
    {
        var lf = new LoggerFactory();
        var experiment = new SimpleExperiment(
            _shufflerMock.Object,
            lf.CreateLogger<SimpleExperiment>(),
            new[]{_elonMock.Object, _markMock.Object});
        
        var result = experiment.Do(_deckMock.Object);
        
        Assert.DoesNotThrow(() =>
        {
            _elonMock.Verify(e => e.Choose(It.IsAny<CardDeck>()), Times.Once);
            _markMock.Verify(e => e.Choose(It.IsAny<CardDeck>()), Times.Once);
        });
    }
    
    [Test]
    public void SimpleExperiment_Has_ExpectedResult()
    {
        var lf = new LoggerFactory();
        var experiment = new SimpleExperiment(
            _shufflerMock.Object,
            lf.CreateLogger<SimpleExperiment>(),
            new[]{_elonMock.Object, _markMock.Object});
        
        var result = experiment.Do(_deckMock.Object);
        
        Assert.That(result, Is.EqualTo(_firstAfterSplit[0].Color == _secondAfterSplit[0].Color));
    }
}
