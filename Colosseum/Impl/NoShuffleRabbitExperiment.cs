﻿using System.Text.Json.Serialization;
using CardLibrary;
using Colosseum.Abstractions;
using Colosseum.Client;
using Colosseum.Exceptions;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedTransitLibrary;

namespace Colosseum.Impl;

public class NoShuffleRabbitExperiment : IExperiment
{
    private readonly TellCardIndexProducer _producer;
    private readonly ILogger<NoShuffleRabbitExperiment> _logger;
    private readonly HttpPlayerAsker<AskForExperimentDto, ColorResponse> _firstAsker;
    private readonly HttpPlayerAsker<AskForExperimentDto, ColorResponse> _secondAsker;
    private readonly Uri _firstMqUri;
    private readonly Uri _secondMqUri;

    public NoShuffleRabbitExperiment(
        ILogger<NoShuffleRabbitExperiment> logger,
        ExperimentConfig config, 
        TellCardIndexProducer producer)
    {
        _producer = producer;
        _logger = logger;
        if (config.Uris.Count != 4)
        {
            throw new NotEnoughPlayersException($"expected 2 player's uris, have {config.Uris.Count}");
        }

        _firstAsker = new HttpPlayerAsker<AskForExperimentDto, ColorResponse>(config.Uris[0]);
        _secondAsker = new HttpPlayerAsker<AskForExperimentDto, ColorResponse>(config.Uris[1]);
        _firstMqUri = config.Uris[2];
        _secondMqUri = config.Uris[3];
    }
    
    
    public bool Do(ShuffleableCardDeck deck)
    {
        deck.Split(out var first, out var second);
        var id = NewId.NextGuid();
        var mqT1 = _producer.SendDeck(id, first, _firstMqUri);
        var mqT2 = _producer.SendDeck(id, second, _secondMqUri);
        Task.WaitAll(mqT1, mqT2);
        var httpT1 = _firstAsker.Ask(new AskForExperimentDto { ExperimentId = id });
        var httpT2 = _secondAsker.Ask(new AskForExperimentDto { ExperimentId = id });
        _logger.LogInformation($"Experiment participants {httpT1.Result.Name} and {httpT2.Result.Name}");
        return httpT1.Result.Color == httpT2.Result.Color;
    }
}

internal class AskForExperimentDto
{
    [JsonPropertyName("ExperimentId")]
    public Guid ExperimentId { get; set; }
}

internal class ColorResponse
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }
    
    [JsonPropertyName("Color")]
    public CardColor Color { get; set; }
}
