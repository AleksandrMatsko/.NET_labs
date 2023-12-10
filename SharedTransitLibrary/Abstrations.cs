using CardLibrary;

namespace SharedTransitLibrary;

public interface ICardIndexToldHandler
{
    void Handle(CardIndexTold message);
}

public interface ITellCardIndexStorage
{
    void AddExperiment(Guid experimentId, CardDeck deck);
    CardDeck? GetExperiment(Guid experimentId);
}
