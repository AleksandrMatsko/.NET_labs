namespace PlayersWebApp;

public class PlayerChoice
{
    public required string Name { get; set; }
    public int CardNumber { get; set; }
    
    public IEnumerable<string>? Errors { get; set; }
}
