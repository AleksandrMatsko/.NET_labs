using Colosseum.Abstractions;
using StrategyLibrary.Interfaces;

namespace Colosseum.Impl;

public class ElonMask : AbstractPlayer
{
    public ElonMask(ICardPickStrategy strategy) : base(strategy)
    {
    }

    public override string Name => "Elon Mask";
}

public class MarkZuckerberg : AbstractPlayer
{
    public MarkZuckerberg(ICardPickStrategy strategy) : base(strategy)
    {
    }

    public override string Name => "Mark Zuckerberg";
}