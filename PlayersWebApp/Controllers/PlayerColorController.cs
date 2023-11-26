using MassTransit.Initializers;
using Microsoft.AspNetCore.Mvc;
using PlayerLibrary;
using PlayersWebApp.Exceptions;
using SharedTransitLibrary;

namespace PlayersWebApp.Controllers;

[ApiController]
[Route("player/color")]
public class PlayerColorController : ControllerBase
{
    private readonly AbstractPlayer _player;
    private readonly CardIndexToldService _service;
    private readonly ITellCardIndexStorage _storage;
    private readonly ILogger<PlayerColorController> _logger;

    public PlayerColorController(
        CardIndexToldService service, 
        AbstractPlayer player, 
        ITellCardIndexStorage storage, 
        ILogger<PlayerColorController> logger)
    {
        _service = service;
        _player = player;
        _storage = storage;
        _logger = logger;
    }
    
    [ProducesResponseType(typeof(PlayerColor), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    [HttpPost(Name = "color")]
    public async Task<IActionResult> Choose([FromBody] AskForColorDto dto)
    {
        var deck = _storage.GetExperiment(dto.ExperimentId);
        if (deck == null)
        {
            _logger.LogWarning($"experiment with id {dto.ExperimentId} not found");
            throw new ExperimentNotFoundException();
        }
        Console.WriteLine("\nbefore Select in PlayerColorController\n");
        var msgTask = _service.AwaitMessageAsync(dto.ExperimentId).
            WaitAsync(TimeSpan.FromSeconds(180)).
            Select(msg => 
            {
                if (msg.Index < 0 || msg.Index >= deck.Length)
                {
                    _logger.LogWarning($"experiment with id {dto.ExperimentId} invalid index {msg.Index} from partner");
                    throw new InvalidIndexFromPartnerException();
                } 
                return Ok(new PlayerColor { Name = _player.Name, Color = deck[msg.Index].Color});
            });
        Console.WriteLine("\nawait\n");
        return await msgTask;
    }
}
