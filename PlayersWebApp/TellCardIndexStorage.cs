using System.Collections.Concurrent;
using CardLibrary;
using SharedTransitLibrary;

namespace PlayersWebApp;

public class TellCardIndexStorage : ITellCardIndexStorage
{
    private readonly IDictionary<Guid, CardDeck> _decks = new ConcurrentDictionary<Guid, CardDeck>();
    
    public void AddExperiment(Guid experimentId, CardDeck deck)
    {
        _decks.Add(experimentId, deck);
    }

    public CardDeck? GetExperiment(Guid experimentId)
    {
        var ok = _decks.TryGetValue(experimentId, out var deck);
        return ok ? deck : null;
    }
}
