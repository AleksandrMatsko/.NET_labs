using CardLibrary;
using Colosseum.Abstractions;
using Microsoft.AspNetCore.Mvc;

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
    
    [HttpPost(Name = "choose")]
    public PlayerChoice Choose([FromBody] IEnumerable<CardDto> cardDtos)
    {
        var cardList = new List<Card>();
        foreach (var dto in cardDtos)
        {
            cardList.Add(dto.ToCard());
        }

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
    public CardColor Color { get; set; }
    public int Number { get; set; }

    public Card ToCard()
    {
        return new Card(Color, Number);
    }
}
