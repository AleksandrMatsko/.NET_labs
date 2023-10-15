using System.Text.Json.Serialization;
using CardLibrary;
using Colosseum.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace PlayersWebApp.Controllers;

[ApiController]
[Route("player")]
public class PlayerChoiceController : ControllerBase
{
    private readonly AbstractPlayer _player;
    private readonly ILogger<PlayerChoiceController> _logger;

    public PlayerChoiceController(AbstractPlayer player, ILogger<PlayerChoiceController> logger)
    {
        _player = player;
        _logger = logger;
    }
    
    // TODO: validation
    [HttpPost(Name = "choose")]
    public PlayerChoice Choose([FromBody] IEnumerable<CardDto> cardDtos)
    {
        var cardList = new List<Card>();
        foreach (var dto in cardDtos)
        {
            cardList.Add(dto.ToCard());
        }
        _logger.LogInformation($"cards received: {cardList.Count}");

        var deck = new CardDeck(cardList);
        return new PlayerChoice
        {
            Name = _player.Name,
            CardNumber = _player.Choose(deck)
        };
    }
}

public class CardDto
{
    [JsonPropertyName("color")]
    [JsonRequired]
    public CardColor Color { get; set; }
    
    [JsonPropertyName("number")]
    [JsonRequired]
    public int Number { get; set; }

    public Card ToCard()
    {
        return new Card(Color, Number);
    }
}
