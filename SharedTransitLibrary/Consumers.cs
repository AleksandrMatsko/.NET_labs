using MassTransit;
using Microsoft.Extensions.Logging;
using PlayerLibrary;

namespace SharedTransitLibrary;

public class TellCardIndexConsumer : IConsumer<TellCardIndexToPartner>
{
    private readonly AbstractPlayer _player;
    private readonly ILogger<TellCardIndexConsumer> _logger;
    private readonly ITellCardIndexStorage _storage;

    public TellCardIndexConsumer(
        ILogger<TellCardIndexConsumer> logger, 
        AbstractPlayer player, ITellCardIndexStorage storage)
    {
        _logger = logger;
        _player = player;
        _storage = storage;
    }

    public async Task Consume(ConsumeContext<TellCardIndexToPartner> context)
    {
        _logger.LogInformation(
            $"TellCardIndexConsumer consumed message with ExperimentId: {context.Message.ExperimentId}");
        var deck = context.Message.ToCardDeck();
        var indx = _player.Choose(deck);
        _storage.AddExperiment(context.Message.ExperimentId, deck);
        await context.Publish(new CardIndexTold
        {
            ExperimentId = context.Message.ExperimentId, 
            Index = indx,
            Name = _player.Name
        }, context.CancellationToken);
    }
}

public class CardIndexToldConsumer : IConsumer<CardIndexTold>
{
    private readonly ILogger<TellCardIndexConsumer> _logger;
    private readonly ICardIndexToldHandler _handler;

    public CardIndexToldConsumer(ILogger<TellCardIndexConsumer> logger, ICardIndexToldHandler handler)
    {
        _logger = logger;
        _handler = handler;
    }

    public Task Consume(ConsumeContext<CardIndexTold> context)
    {
        _logger.LogInformation(
            $"CardIndexToldConsumer consumed message with ExperimentId: {context.Message.ExperimentId}, Index: {context.Message.Index} from {context.Message.Name}");
        _handler.Handle(context.Message);
        return Task.CompletedTask;
    }
}

