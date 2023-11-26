using MassTransit;
using Microsoft.Extensions.Logging;
using PlayerLibrary;

namespace SharedTransitLibrary;

public class TellCardIndexConsumer : IConsumer<TellCardIndexToPartner>
{
    private readonly AbstractPlayer _player;
    private readonly ILogger<TellCardIndexConsumer> _logger;

    public TellCardIndexConsumer(
        ILogger<TellCardIndexConsumer> logger, 
        AbstractPlayer player)
    {
        _logger = logger;
        _player = player;
    }

    public async Task Consume(ConsumeContext<TellCardIndexToPartner> context)
    {
        _logger.LogInformation(
            $"TellCardIndexConsumer consumed message with RequestId: {context.Message.RequestId}");
        var deck = context.Message.ToCardDeck();
        var indx = _player.Choose(deck);
        await context.Publish(new CardIndexTold
        {
            RequestId = context.Message.RequestId, 
            Index = indx,
            Name = _player.Name
        }, context.CancellationToken);
    }
}

public class CardIndexToldConsumer : IConsumer<CardIndexTold>
{
    private readonly ILogger<TellCardIndexConsumer> _logger;
    private readonly AbstractPlayer _player;


    public CardIndexToldConsumer(ILogger<TellCardIndexConsumer> logger, AbstractPlayer player)
    {
        _logger = logger;
        _player = player;
    }

    public Task Consume(ConsumeContext<CardIndexTold> context)
    {
        if (context.Message.Name == _player.Name)
        {
            return Task.CompletedTask;
        }
        _logger.LogInformation(
            $"CardIndexToldConsumer consumed message with RequestId: {context.Message.RequestId}, Index: {context.Message.Index} from {context.Message.Name}");
        return Task.CompletedTask;
    }
}

