using Blackjack.Domain.Betting;
using Blackjack.Domain.Exceptions;

namespace Blackjack.Domain.Rules;

public sealed record RuleSet
{
    public static RuleSet Bicycle { get; } = new();

    // Shoe
    public int DeckCount { get; init; } = 6;
    public double Penetration { get; init; } = 0.75;

    // Dealer
    public bool DealerHitsSoft17 { get; init; }

    // Payouts
    public int BlackjackPayoutNumerator { get; init; } = 3;
    public int BlackjackPayoutDenominator { get; init; } = 2;
    public int InsurancePayoutRatio { get; init; } = 2;

    // Player options
    // Bicycle: double down when the first two cards total 9, 10, or 11.
    public IReadOnlySet<int> DoubleDownTotals { get; init; } = new HashSet<int> { 9, 10, 11 };
    public bool AllowDoubleAfterSplit { get; init; } = true;

    // Table Limits
    public Chips StartingBankroll { get; init; } = new(1000);
    public Chips MinimumBet { get; init; } = new(10);
    public Chips MaximumBet { get; init; } = new(500);

    public int BetIncrement { get; init; } = 10;

    public void ValidateBet(Chips bet)
    {
        if (bet < MinimumBet || bet > MaximumBet)
            throw new InvalidBetException($"Bets must be between {MinimumBet} and {MaximumBet}");

        if (bet.Amount % BetIncrement != 0) throw new InvalidBetException($"Bets must be a multiple of {BetIncrement}");
    }
}