using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CardLibrary;
using CardLibrary.Abstractions;
using Colosseum.Abstractions;
using Colosseum.Exceptions;
using Microsoft.Extensions.Logging;

namespace Colosseum.Impl;

public class SimpleExperiment : IExperiment
{
    private readonly AbstractPlayer _firstPlayer;
    private readonly AbstractPlayer _secondPlayer;
    private readonly IDeckShuffler _deckShuffler;
    private readonly ShuffleableCardDeck _cardDeck;

    public SimpleExperiment(
        ShuffleableCardDeck cardDeck,
        IDeckShuffler deckShuffler, 
        ILogger<SimpleExperiment> logger, 
        IEnumerable<AbstractPlayer> players)
    {
        var enumerable = players as AbstractPlayer[] ?? players.ToArray();
        if (enumerable.Length < 2)
        {
            throw new NotEnoughPlayersException($"expected 2 players, have {enumerable.Length}");
        }
        _firstPlayer = enumerable[0];
        _secondPlayer = enumerable[1];
        _cardDeck = cardDeck;
        _deckShuffler = deckShuffler;

        logger.LogInformation($"Experiment participants: {_firstPlayer.Name} and {_secondPlayer.Name}");
    }
    
    public bool Do()
    {
        _deckShuffler.Shuffle(_cardDeck);
        
        _cardDeck.Split(out var firstDeck, out var secondDeck);
        
        var firstChoice = _firstPlayer.Choose(firstDeck);
        var secondChoice = _secondPlayer.Choose(secondDeck);
        return firstDeck[secondChoice].Color == secondDeck[firstChoice].Color;
    }
}

public class WithHttpExperiment : IExperiment
{
    private readonly ILogger<WithHttpExperiment> _logger;
    private readonly Uri[] _uris;
    private readonly IDeckShuffler _deckShuffler;
    private readonly ShuffleableCardDeck _cardDeck;

    public WithHttpExperiment(
        ILogger<WithHttpExperiment> logger, 
        IEnumerable<Uri> uris, 
        IDeckShuffler deckShuffler, 
        ShuffleableCardDeck cardDeck)
    {
        _logger = logger;
        _uris = uris as Uri[] ?? uris.ToArray();
        _deckShuffler = deckShuffler;
        _cardDeck = cardDeck;
    }

    private IEnumerable<CardDto> ConvertDeck(CardDeck deck)
    {
        var dtos = new CardDto[deck.Length];
        for (var i = 0; i < deck.Length; i++)
        {
            dtos[i] = new CardDto(deck[i]);
        }
        return dtos;
    }

    private async Task<CardChoiceDto> AskPlayer(Uri uri, CardDeck deck)
    {
        var httpClient = new HttpClient();
        var postContent = JsonContent.Create(ConvertDeck(deck));
        var response = await httpClient.PostAsync(uri, postContent);
        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<CardChoiceDto>(stream) ?? throw new InvalidOperationException();
    }

    public bool Do()
    {
        _deckShuffler.Shuffle(_cardDeck);
        
        _cardDeck.Split(out var firstDeck, out var secondDeck);

        var t1 = AskPlayer(_uris[0], firstDeck);
        var t2 = AskPlayer(_uris[1], secondDeck);

        Task.WaitAll(t1, t2);
        
        _logger.LogInformation($"Experiment participants: {t1.Result.Name} -> {t1.Result.CardNumber} and {t2.Result.Name} -> {t2.Result.CardNumber}");
        return firstDeck[t2.Result.CardNumber].Color == secondDeck[t1.Result.CardNumber].Color;
    }
}

public class CardChoiceDto
{
    [JsonPropertyName("name")]
    [JsonRequired]
    public string Name { get; set; }
    
    [JsonPropertyName("cardNumber")]
    [JsonRequired]
    public int CardNumber { get; set; }

    [JsonPropertyName("errors")]
    public IEnumerable<string> Errors { get; set; }
}

public class CardDto
{
    [JsonPropertyName("Color")]
    public CardColor Color { get; set; }
    
    [JsonPropertyName("Number")]
    public int Number { get; set; }

    public CardDto(Card card)
    {
        Color = card.Color;
        Number = card.Number;
    }
}