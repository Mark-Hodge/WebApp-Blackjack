using Blackjack.Domain.Cards;
using Blackjack.Domain.Rules;

namespace Blackjack.Domain.Hands;

public sealed class DealerHand : Hand
{
    public Card UpCard => Cards[0];

    public bool IsHoleCardRevealed { get; private set; }

    public IEnumerable<Card> VisibleCards => IsHoleCardRevealed ? Cards : Cards.Take(1);

    public HandValue VisibleValue => HandValue.Of(VisibleCards);

    public bool ShouldPeekForNatural => UpCard.IsAce || UpCard.IsTenCard;

    internal void RevealHoleCard()
    {
        IsHoleCardRevealed = true;
    }

    // Bicycle: hit on 16 or under, stand on 17 or more. An ace counts as 11 whenever that makes 17–21,
    // so the dealer stands on soft 17 unless the rule set says otherwise.
    internal bool MustHit(RuleSet rules)
    {
        return Total < 17 || (rules.DealerHitsSoft17 && Total == 17 && IsSoft);
    }
}