using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CardLibrary;
using Colosseum.Exceptions;
using Microsoft.Extensions.Logging;

namespace Colosseum.Client;

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