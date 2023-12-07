using CardLibrary;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SharedTransitLibrary;

public class TellCardIndexProducer
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<TellCardIndexProducer> _logger;

    public TellCardIndexProducer(ISendEndpointProvider sendEndpointProvider, IHostApplicationLifetime lifetime, ILogger<TellCardIndexProducer> logger)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _lifetime = lifetime;
        _logger = logger;
    }

    public async Task SendDeck(Guid experimentId, CardDeck deck, Uri uri)
    {
        var dtos = new List<TransitCardDto>();
        for (int i = 0; i < deck.Length; i++)
        {
            dtos.Add(new TransitCardDto { Color = deck[i].Color, Number = deck[i].Number });
        }
        
        _logger.LogInformation($"sending to {uri} experiment with id {experimentId}");
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(uri);
        await sendEndpoint.Send(new TellCardIndex
        {
            ExperimentId = experimentId, 
            CardDtos = dtos
        }, _lifetime.ApplicationStopping);
    }
}
