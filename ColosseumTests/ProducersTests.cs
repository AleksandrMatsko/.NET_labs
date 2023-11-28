using CardLibrary;
using ColosseumTests.DeckTests;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using PlayerLibrary;
using SharedTransitLibrary;
using StrategyLibrary.Impl;

namespace ColosseumTests;

[TestFixture]
public class ProducersTests
{
    private Mock<ITellCardIndexStorage> _storageMock;
    private AbstractPlayer _player;
    
    [SetUp]
    public void SetUp()
    {
        _storageMock = new Mock<ITellCardIndexStorage>();
        _storageMock.Setup(s => s.AddExperiment(It.IsAny<Guid>(), It.IsAny<CardDeck>()));
        _player = new ElonMask(new PickFirstCardStrategy());
    }
    
    [Test]
    public async Task TellCardIndexProducer_SendsMsgAndMsgIsConsumed()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<TellCardIndexConsumer>();
            })
            .AddSingleton(_storageMock.Object)
            .AddSingleton(_player)
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        var endpoint = await harness.GetConsumerEndpoint<TellCardIndexConsumer>();
        var endpointProviderMock = new Mock<ISendEndpointProvider>();
        endpointProviderMock.Setup(p => p.GetSendEndpoint(It.IsAny<Uri>()))
            .Returns(Task.FromResult(endpoint));
        var lf = new LoggerFactory();
        var lifetime = new ApplicationLifetime(new Logger<ApplicationLifetime>(lf));
        var experimentId = Guid.NewGuid();
        var deck = new CardDeck(CardDeckUtils.PrepareCards(18));
        
        var producer = new TellCardIndexProducer(endpointProviderMock.Object, lifetime);

        await producer.SendDeck(experimentId, deck, new Uri("queue:SharedTransitLibrary.TellCardIndex"));
        
        Assert.Multiple(async () =>
        {
            Assert.That(await harness.Sent.Any<TellCardIndex>(), Is.True);
            Assert.That(await harness.Consumed.Any<TellCardIndex>(), Is.True);
        });
        Assert.DoesNotThrow(() =>
        {
            endpointProviderMock.Verify(p => p.GetSendEndpoint(It.IsAny<Uri>()), Times.Once);
        });
        
        var consumerHarness = harness.GetConsumerHarness<TellCardIndexConsumer>();
        Assert.That(await consumerHarness.Consumed.Any<TellCardIndex>(
            x =>
            {
                var ok = experimentId == x.Context.Message.ExperimentId;
                var receivedDeck = x.Context.Message.ToCardDeck();
                if (!ok || deck.Length != receivedDeck.Length)
                {
                    return false;
                }

                for (var i = 0; i < deck.Length; i++)
                {
                    ok = ok && deck[i].Color == receivedDeck[i].Color && deck[i].Number == receivedDeck[i].Number;
                    if (!ok)
                    {
                        return false;
                    }
                }

                return true;
            }));
    }
}
