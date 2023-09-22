namespace CardLibrary.Abstractions;

public interface IDeckShuffler
{
    void Shuffle(ShuffleableCardDeck cardDeck);
}