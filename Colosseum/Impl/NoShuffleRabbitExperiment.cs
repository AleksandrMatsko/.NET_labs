using CardLibrary;
using Colosseum.Abstractions;
using Colosseum.Exceptions;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedTransitLibrary;

namespace Colosseum.Impl;

public class NoShuffleRabbitExperiment : IExperiment
{
    private readonly TellCardIndexProducer _producer;
    private readonly ILogger<NoShuffleRabbitExperiment> _logger;

    public NoShuffleRabbitExperiment(
        ILogger<NoShuffleRabbitExperiment> logger,
        ExperimentConfig config, 
        TellCardIndexProducer producer)
    {
        _producer = producer;
        _logger = logger;
    }
    
    
    public bool Do(ShuffleableCardDeck deck)
    {
        deck.Split(out var first, out var second);
        var id = NewId.NextGuid();
        var t1 = _producer.SendDeck(id, first, new Uri("queue:Elon.SharedTransitLibrary.TellCardIndex"));
        var t2 = _producer.SendDeck(id, second, new Uri("queue:Mark.SharedTransitLibrary.TellCardIndex"));
        Task.WaitAll(t1, t2);
        return false;
    }
}
