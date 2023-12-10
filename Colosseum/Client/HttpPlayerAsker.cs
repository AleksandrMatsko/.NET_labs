using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CardLibrary;
using Colosseum.Exceptions;
using Microsoft.Extensions.Logging;

namespace Colosseum.Client;

public class HttpPlayerAsker<TReq, TResp>
{
    private readonly Uri _uri;

    public HttpPlayerAsker(Uri uri)
    {
        _uri = uri;
    }
    
    public async Task<TResp> Ask(TReq data)
    {
        var httpClient = new HttpClient();
        var postContent = JsonContent.Create(data);
        var response = await httpClient.PostAsync(_uri, postContent);
        var stream = await response.Content.ReadAsStreamAsync();
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
            {
                break;
            }
            default:
                throw new UnexpectedHttpStatusCodeException($"unexpected status code {response.StatusCode}");
        }
        var result = await JsonSerializer.DeserializeAsync<TResp>(stream) ?? throw new InvalidOperationException();
        return result;
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
