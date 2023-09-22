using CardLibrary;

namespace StrategyLibrary.Interfaces;

public interface ICardPickStrategy
{
    int Choose(in CardDeck cardDeck);
}