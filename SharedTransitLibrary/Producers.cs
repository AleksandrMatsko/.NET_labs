using CardLibrary;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace SharedTransitLibrary;

public class TellCardIndexProducer
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IHostApplicationLifetime _lifetime;

    public TellCardIndexProducer(ISendEndpointProvider sendEndpointProvider, IHostApplicationLifetime lifetime)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _lifetime = lifetime;
    }

    public async Task SendDeck(Guid requestId, CardDeck deck, Uri uri)
    {
        var dtos = new List<TransitCardDto>();
        for (int i = 0; i < deck.Length; i++)
        {
            dtos.Add(new TransitCardDto { Color = deck[i].Color, Number = deck[i].Number });
        }

        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(uri);
        await sendEndpoint.Send(new TellCardIndexToPartner
        {
            ExperimentId = requestId, 
            CardDtos = dtos
        }, _lifetime.ApplicationStopping);
    }
}
