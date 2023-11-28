using CardLibrary;

namespace SharedTransitLibrary;

public class TellCardIndex
{
    public Guid ExperimentId { get; set; }
    public IList<TransitCardDto> CardDtos { get; set; }

    public CardDeck ToCardDeck()
    {
        var cardsList = new List<Card>();
        foreach (var dto in CardDtos)
        {
            cardsList.Add(new Card(dto.Color, dto.Number));    
        }

        return new CardDeck(cardsList);
    }
}
