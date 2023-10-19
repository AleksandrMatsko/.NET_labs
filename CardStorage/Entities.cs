using CardLibrary;

namespace CardStorage;

public class CardEntity
{
    public int Id { get; set; }
    public CardColor Color { get; set; }
    public int Number { get; set; }

    public CardEntity(CardColor color, int number)
    {
        Color = color;
        Number = number;
    }
    
    public CardEntity(Card card)
    {
        Color = card.Color;
        Number = card.Number;
    }

    public static CardEntity FromCard(Card card)
    {
        return new CardEntity(card);
    }

    public Card ToCard()
    {
        return new Card(Color, Number);
    }
}

public class ExperimentCondition
{
    public int Id { get; set; }
    public IList<CardEntity> CardEntities { get; set; }
    
    public ExperimentCondition() {}

    public ExperimentCondition(IList<CardEntity> entities)
    {
        CardEntities = entities;
    }

    public ExperimentCondition(CardDeck deck)
    {
        CardEntities = (from Card card in deck select CardEntity.FromCard(card)).ToList();
    }

    public static ExperimentCondition FromDeck(CardDeck deck)
    {
        return new ExperimentCondition(deck);
    }

    public CardDeck ToDeck()
    {
        return new CardDeck(CardEntities.Select(c => c.ToCard()).ToList());
    }
}