using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CardLibrary;
using CardLibrary.Abstractions;
using Colosseum.Abstractions;
using Colosseum.Exceptions;
using Microsoft.Extensions.Logging;

namespace Colosseum.Impl;

public class HttpExperiment : IExperiment
{
    private readonly ILogger<HttpExperiment> _logger;
    private readonly HttpPlayerAsker _elonAsker;
    private readonly HttpPlayerAsker _markAsker;
    private readonly IDeckShuffler _deckShuffler;
    private readonly ShuffleableCardDeck _cardDeck;

    public HttpExperiment(
        ILogger<HttpExperiment> logger, 
        IEnumerable<Uri> uris, 
        IDeckShuffler deckShuffler, 
        ShuffleableCardDeck cardDeck)
    {
        _logger = logger;
        var urisArr = uris as Uri[] ?? uris.ToArray();
        if (urisArr.Length < 2)
        {
            throw new NotEnoughPlayersException($"expected 2 players, have {urisArr.Length}");
        }

        var lf = new LoggerFactory();
        _elonAsker = new HttpPlayerAsker(urisArr[0], new Logger<HttpPlayerAsker>(lf));
        _markAsker = new HttpPlayerAsker(urisArr[0], new Logger<HttpPlayerAsker>(lf));
        _deckShuffler = deckShuffler;
        _cardDeck = cardDeck;
    }
    
    public bool Do()
    {
        _deckShuffler.Shuffle(_cardDeck);
        
        _cardDeck.Split(out var firstDeck, out var secondDeck);

        var t1 = _elonAsker.Ask(firstDeck);
        var t2 = _markAsker.Ask(secondDeck);

        Task.WaitAll(t1, t2);
        
        _logger.LogInformation($"Experiment participants: {t1.Result.Name} -> {t1.Result.CardNumber} and {t2.Result.Name} -> {t2.Result.CardNumber}");
        return firstDeck[t2.Result.CardNumber].Color == secondDeck[t1.Result.CardNumber].Color;
    }
}

public class HttpPlayerAsker
{
    private readonly Uri _uri;
    private readonly ILogger<HttpPlayerAsker> _logger;

    public HttpPlayerAsker(Uri uri, ILogger<HttpPlayerAsker> logger)
    {
        _uri = uri;
        _logger = logger;
    }
    
    public async Task<CardChoiceDto> Ask(CardDeck deck)
    {
        var httpClient = new HttpClient();
        var postContent = JsonContent.Create(ConvertDeck(deck));
        var response = await httpClient.PostAsync(_uri, postContent);
        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<CardChoiceDto>(stream) ?? throw new InvalidOperationException();
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
            {
                return result;
            }
            case HttpStatusCode.BadRequest:
                throw new BadRequestException($"incorrect request, errors: {string.Join(", ", result.Errors)}");
            case HttpStatusCode.InternalServerError:
                throw new ServerErrorException("server error");
            default:
                throw new UnexpectedHttpStatusCodeException($"unexpected status code {response.StatusCode}");
        }
        
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