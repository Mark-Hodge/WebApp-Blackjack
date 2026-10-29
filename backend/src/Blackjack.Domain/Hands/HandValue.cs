using Blackjack.Domain.Cards;

namespace Blackjack.Domain.Hands;

public readonly record struct HandValue(int Total, bool IsSoft)
{
    public bool IsBust => Total > 21;

    public static HandValue Of(IEnumerable<Card> cards)
    {
        var hardTotal = 0;
        var hasAce = false;
        foreach (var card in cards)
        {
            hardTotal += card.Points;
            hasAce |= card.IsAce;
        }

        var canCountAceAsEleven = hasAce && hardTotal + 10 <= 21;
        return canCountAceAsEleven
            ? new HandValue(hardTotal + 10, true)
            : new HandValue(hardTotal, false);
    }
}