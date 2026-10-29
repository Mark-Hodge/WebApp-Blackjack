using Blackjack.Domain.Betting;
using Blackjack.Domain.Hands;
using Blackjack.Domain.Rules;

namespace Blackjack.Domain.Rounds;

public static class Settlement
{
    public static HandOutcome DetermineOutcome(PlayerHand player, DealerHand dealer)
    {
        if (player.IsBust) return HandOutcome.Loss;
        if (player.IsNatural && dealer.IsNatural) return HandOutcome.Push;
        if (player.IsNatural) return HandOutcome.Win;
        if (dealer.IsNatural) return HandOutcome.Loss;
        if (dealer.IsBust) return HandOutcome.Win;

        return player.Total.CompareTo(dealer.Total) switch
        {
            > 0 => HandOutcome.Win,
            0 => HandOutcome.Push,
            _ => HandOutcome.Loss
        };
    }

    public static HandResult Settle(PlayerHand player, DealerHand dealer, RuleSet rules)
    {
        var outcome = DetermineOutcome(player, dealer);
        var bet = player.Bet;

        var payout = outcome switch
        {
            HandOutcome.Win when player.IsNatural =>
                bet + bet.MultiplyBy(rules.BlackjackPayoutNumerator, rules.BlackjackPayoutDenominator),
            HandOutcome.Win => bet * 2,
            HandOutcome.Push => bet,
            _ => Chips.Zero
        };

        return new HandResult(outcome, bet, payout);
    }
}