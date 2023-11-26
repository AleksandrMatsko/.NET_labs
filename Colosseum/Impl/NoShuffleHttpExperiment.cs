using CardLibrary;
using Colosseum.Abstractions;
using Colosseum.Client;
using Colosseum.Exceptions;
using Microsoft.Extensions.Logging;

namespace Colosseum.Impl;

public class NoShuffleHttpExperiment : IExperiment
{
    private readonly ILogger<NoShuffleHttpExperiment> _logger;
    private readonly HttpPlayerAsker<IEnumerable<CardDto>, CardChoiceDto> _firstAsker;
    private readonly HttpPlayerAsker<IEnumerable<CardDto>, CardChoiceDto> _secondAsker;

    public NoShuffleHttpExperiment(
        ILogger<NoShuffleHttpExperiment> logger, 
        ExperimentConfig config)
    {
        _logger = logger;
        if (config.Uris.Count < 2)
        {
            throw new NotEnoughPlayersException($"expected 2 player's uris, have {config.Uris.Count}");
        }
        
        _firstAsker = new HttpPlayerAsker<IEnumerable<CardDto>, CardChoiceDto>(config.Uris[0]);
        _secondAsker = new HttpPlayerAsker<IEnumerable<CardDto>, CardChoiceDto>(config.Uris[1]);
    }
    
    public bool Do(ShuffleableCardDeck cardDeck)
    {
        cardDeck.Split(out var firstDeck, out var secondDeck);

        var t1 = _firstAsker.Ask(ConvertDeck(firstDeck));
        var t2 = _secondAsker.Ask(ConvertDeck(secondDeck));
        
        _logger.LogInformation($"Experiment participants: {t1.Result.Name} -> {t1.Result.CardNumber} and {t2.Result.Name} -> {t2.Result.CardNumber}");
        return firstDeck[t2.Result.CardNumber].Color == secondDeck[t1.Result.CardNumber].Color;
    }
    
    private static IEnumerable<CardDto> ConvertDeck(CardDeck deck)
    {
        var dtos = new CardDto[deck.Length];
        for (var i = 0; i < deck.Length; i++)
        {
            dtos[i] = new CardDto(deck[i]);
        }
        return dtos;
    }
}
