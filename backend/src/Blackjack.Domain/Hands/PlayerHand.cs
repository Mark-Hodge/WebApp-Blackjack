using Blackjack.Domain.Betting;
using Blackjack.Domain.Rounds;

namespace Blackjack.Domain.Hands;

public sealed class PlayerHand : Hand
{
    internal PlayerHand(Chips bet, bool isFromSplit = false)
    {
        Bet = bet;
        IsFromSplit = isFromSplit;
    }

    public Chips Bet { get; private set; }
    public bool IsDoubled { get; private set; }
    public bool IsFromSplit { get; private set; }
    public bool IsComplete { get; private set; }
    public HandResult? Result { get; private set; }
    public override bool IsNatural => !IsFromSplit && base.IsNatural;
    public bool IsPair => Cards.Count == 2 && Cards[0].Rank == Cards[1].Rank;
    public bool IsSplitAces => IsFromSplit && Cards[0].IsAce;

    internal void Complete()
    {
        IsComplete = true;
    }

    internal void DoubleBet()
    {
        Bet *= 2;
        IsDoubled = true;
    }

    internal PlayerHand Split()
    {
        IsFromSplit = true;
        var newHand = new PlayerHand(Bet, true);
        newHand.Add(RemoveLastCard());
        return newHand;
    }

    internal void Settle(HandResult result)
    {
        Result = result;
    }
}