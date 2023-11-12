using System.Text.Json.Serialization;
using CardLibrary;

namespace PlayersWebApp;

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

public class PlayerChoice
{
    public required string Name { get; set; }
    public int CardNumber { get; set; }
}

public class ErrorResponse
{
    public IList<ErrorDto> Errors { get; set; }
}

public class ErrorDto
{
    public string ErrType { get; set; }
}
