using CardLibrary;

namespace Colosseum.Abstractions;

public interface IExperiment
{
    bool Do(ShuffleableCardDeck deck);
}