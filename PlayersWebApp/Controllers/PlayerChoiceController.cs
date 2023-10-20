using System.Net;
using System.Text.Json.Serialization;
using CardLibrary;
using Colosseum.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PlayersWebApp.Validators;

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
    
    [ProducesResponseType(typeof(PlayerChoice), 200)]
    [ProducesResponseType(typeof(PlayerChoice), 400)]
    [HttpPost(Name = "choose")]
    public IActionResult Choose([FromBody] IList<CardFromClientDto> dtos)
    {
        /*var request = HttpContext.Request;
        using var reader = new StreamReader(request.Body);
        var data = reader.ReadToEndAsync();*/
        var deck = CardDeckValidator.ValidateAndReturn(dtos/*data.Result, out var messages*/);
        if (deck.Length != CardDeckValidator.CardsCount)
        {
            return BadRequest(new PlayerChoice
                { Name = _player.Name, CardNumber = -1, Errors = new[] { "bad deck length" } });
        }

        return Ok(new PlayerChoice { Name = _player.Name, CardNumber = _player.Choose(deck) });
        /*if (deck == null)
        {
            _logger.LogWarning($"response code: {HttpStatusCode.BadRequest}");
            HttpContext.Response.StatusCode = 400;
            return new PlayerChoice()
            {
                Name = _player.Name,
                CardNumber = -1,
                Errors = messages
            };
        }
        _logger.LogInformation($"response code: {HttpStatusCode.OK}");
        HttpContext.Response.StatusCode = 200;
        return new PlayerChoice
        {
            Name = _player.Name,
            CardNumber = _player.Choose(deck)
        };*/
    }
}

public class CardFromClientDto
{
    [JsonPropertyName("Color")]
    [JsonRequired]
    public CardColor Color { get; set; }
    
    [JsonPropertyName("Number")]
    [JsonRequired]
    public int Number { get; set; }

    public Card ToCard()
    {
        return new Card(Color, Number);
    }
}
