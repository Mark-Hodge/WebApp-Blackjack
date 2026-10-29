using Blackjack.Domain.Betting;

namespace Blackjack.Domain.Rounds;

public enum HandOutcome
{
    Win,
    Loss,
    Push
}

public sealed record HandResult(HandOutcome Outcome, Chips Wagered, Chips Payout)
{
    public int NetChips => Payout.Amount - Wagered.Amount;
}