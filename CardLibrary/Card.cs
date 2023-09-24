namespace CardLibrary;

public class Card
{
    public Card(CardColor color, int num)
    {
        Color = color;
        Number = num;
    }

    public CardColor Color { get; }
    
    public int Number { get; }

    protected bool Equals(Card other)
    {
        return Color == other.Color && Number == other.Number;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Card)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Color, Number);
    }
}

public enum CardColor
{
    Black,
    Red
}