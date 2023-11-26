namespace PlayersWebApp.Exceptions;

public class ClientException : Exception
{
    public ClientException() {}

    public ClientException(string message) : base(message) {}
}

public class BadDeckLength : ClientException
{
    public BadDeckLength() {}

    public BadDeckLength(string message) : base(message) {}
}

public class ExperimentNotFoundException : ClientException
{
    public ExperimentNotFoundException() {}

    public ExperimentNotFoundException(string message) : base(message) {}
}

public class InvalidIndexFromPartnerException : ClientException
{
    public InvalidIndexFromPartnerException() {}

    public InvalidIndexFromPartnerException(string message) : base(message) {}
}

public class ServerException : Exception
{
    public ServerException() {}

    public ServerException(string message) : base(message) {}
}
