using PlayerLibrary;
using SharedTransitLibrary;

namespace PlayersWebApp.RabbitMQ;

public class CardIndexToldHandler : ICardIndexToldHandler
{
    private readonly AbstractPlayer _player;
    private readonly CardIndexToldService _service;


    public CardIndexToldHandler(AbstractPlayer player, CardIndexToldService service)
    {
        _player = player;
        _service = service;
    }

    public void Handle(CardIndexTold message)
    {
        if (message.Name == _player.Name)
        {
            return;
        }
        _service.AddMessage(message);
    }
}
