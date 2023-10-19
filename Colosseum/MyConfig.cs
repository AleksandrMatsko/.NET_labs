namespace Colosseum;

public class MyConfig
{
    public int ExperimentCount { get; init; }
    public DbRequest Request { get; init; }
}

public enum DbRequest
{
    None,
    Generate,
    UseGenerated
}