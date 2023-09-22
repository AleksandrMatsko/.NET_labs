namespace Colosseum;

public class NotEnoughPlayersException : Exception
{
    public NotEnoughPlayersException(string message) : base(message) {}
    
}