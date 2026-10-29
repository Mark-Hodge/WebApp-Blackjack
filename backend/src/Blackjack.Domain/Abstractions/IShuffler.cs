using Blackjack.Domain.Cards;

namespace Blackjack.Domain.Abstractions;

public interface IShuffler
{
    void Shuffle(IList<Card> cards);
}