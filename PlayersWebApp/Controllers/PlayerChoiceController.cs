using Colosseum.Abstractions;
using Microsoft.AspNetCore.Mvc;
using PlayersWebApp.Validators;

namespace PlayersWebApp.Controllers;

[ApiController]
[Route("player")]
public class PlayerChoiceController : ControllerBase
{
    private readonly AbstractPlayer _player;

    public PlayerChoiceController(AbstractPlayer player)
    {
        _player = player;
    }
    
    [ProducesResponseType(typeof(PlayerChoice), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    [HttpPost(Name = "choose")]
    public IActionResult Choose([FromBody] IList<CardFromClientDto> dtos)
    {
        var deck = CardDeckValidator.ValidateAndReturn(dtos);
        
        return Ok(new PlayerChoice { Name = _player.Name, CardNumber = _player.Choose(deck) });
    }
}

