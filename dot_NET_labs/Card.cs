namespace dot_NET_labs;

public class Card
{
    public Card(CardColor color, int num)
    {
        Color = color;
        Number = num;
    }

    public CardColor Color { get; }
    
    public int Number { get; }
}

public enum CardColor
{
    Black,
    Red
}