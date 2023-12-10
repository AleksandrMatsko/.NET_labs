using CardLibrary;
using PlayersWebApp.Controllers;
using PlayersWebApp.Exceptions;

namespace PlayersWebApp.Validators;

public class CardDeckValidator
{
    public static int CardsCount { get; } = 18;
    
    public static CardDeck ValidateAndReturn(IList<CardFromClientDto> dtos)
    {
        var deck = new CardDeck(dtos.Select(dto => dto.ToCard()).ToList());
        if (deck.Length != CardsCount)
        {
            throw new BadDeckLength();
        }

        return deck;
    }
}