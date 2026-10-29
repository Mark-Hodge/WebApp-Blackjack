using Blackjack.Domain.Abstractions;
using Blackjack.Domain.Cards;

namespace Blackjack.Domain.Tests.TestSupport;

internal sealed class StackedShuffler(params Card[] cardsOnTop) : IShuffler
{
    public void Shuffle(IList<Card> cards)
    {
        for (var position = 0; position < cardsOnTop.Length; position++)
        {
            var source = IndexOf(cards, cardsOnTop[position], startAt: position);
            (cards[position], cards[source]) = (cards[source], cards[position]);
        }
    }

    private static int IndexOf(IList<Card> cards, Card card, int startAt)
    {
        for (var index = startAt; index < cards.Count; index++)
        {
            if (cards[index] == card) return index;
        }

        throw new InvalidOperationException($"The shoe has no more copies of {card} to stack.");
    }
}