namespace Colosseum.Exceptions;

public class NotEnoughPlayersException : Exception
{
    public NotEnoughPlayersException(string message) : base(message) {}
}

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) {}
}

public class ServerErrorException : Exception
{
    public ServerErrorException(string message) : base(message) {}
}

public class UnexpectedHttpStatusCodeException : Exception
{
    public UnexpectedHttpStatusCodeException(string message) : base(message) {}
}
