using CardLibrary;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PlayerLibrary;
using SharedTransitLibrary;
using StrategyLibrary.Impl;

namespace ColosseumTests;

[TestFixture]
public class ConsumersTests
{
    private Mock<ITellCardIndexStorage> _storageMock;
    private AbstractPlayer _player;
    private Mock<ICardIndexToldHandler> _handlerMock;
    
    [SetUp]
    public void SetUp()
    {
        _storageMock = new Mock<ITellCardIndexStorage>();
        _storageMock.Setup(s => s.AddExperiment(It.IsAny<Guid>(), It.IsAny<CardDeck>()));
        _player = new ElonMask(new PickFirstCardStrategy());
        _handlerMock = new Mock<ICardIndexToldHandler>();
        _handlerMock.Setup(h => h.Handle(It.IsAny<CardIndexTold>()));
    }
    
    [Test]
    public async Task TellCardIndexConsumer_WhenMsgSent_ConsumesMsgAndPublishesCardIndexTold()
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
        var experimentId = Guid.NewGuid();
        var cardDtos = new List<TransitCardDto>
        {
            new() {Color = CardColor.Black, Number = 0}, 
            new() {Color = CardColor.Red, Number = 1}
        };

        var endpoint = await harness.GetConsumerEndpoint<TellCardIndexConsumer>();
        await endpoint.Send(new TellCardIndex { ExperimentId = experimentId, CardDtos = cardDtos });
        Assert.Multiple(async () =>
        {
            Assert.That(await harness.Sent.Any<TellCardIndex>(), Is.True);
            Assert.That(await harness.Consumed.Any<TellCardIndex>(), Is.True);
            Assert.That(await harness.Published.Any<CardIndexTold>(), Is.True);
        });
        var consumerHarness = harness.GetConsumerHarness<TellCardIndexConsumer>();
        Assert.That(await consumerHarness.Consumed.Any<TellCardIndex>(
            x =>
            {
                return cardDtos.Zip(
                    x.Context.Message.CardDtos, 
                    (before, after) => 
                        before.Color == after.Color && before.Number == after.Number
                        ).All(b => b) && x.Context.Message.ExperimentId == experimentId;
            }));
        
        Assert.DoesNotThrow(() => _storageMock.Verify(
            s => s.AddExperiment(It.IsAny<Guid>(), It.IsAny<CardDeck>()), Times.Once));
    }

    [Test]
    public async Task CardIndexToldConsumer_WhenMsgPublished_Consumes()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<CardIndexToldConsumer>();
            })
            .AddSingleton(_handlerMock.Object)
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        var experimentId = Guid.NewGuid();
        const string name = "John Smith";
        
        await harness.Bus.Publish(new CardIndexTold {ExperimentId = experimentId, Name = name});
        
        Assert.That(await harness.Published.Any<CardIndexTold>(), Is.True);
        Assert.That(await harness.Consumed.Any<CardIndexTold>(), Is.True);
        var consumerHarness = harness.GetConsumerHarness<CardIndexToldConsumer>();
        Assert.That(await consumerHarness.Consumed.Any<CardIndexTold>(
            x =>
            {
                Console.WriteLine($"\nreceived {x.Context.Message.ExperimentId}\n");
                return x.Context.Message.Name == name && x.Context.Message.ExperimentId == experimentId;
            }));
        
        Assert.DoesNotThrow(() => _handlerMock.Verify(
            h => h.Handle(It.IsAny<CardIndexTold>()), Times.Once));
    }
}
