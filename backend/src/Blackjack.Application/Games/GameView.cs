using Blackjack.Domain.Cards;
using Blackjack.Domain.Rounds;

namespace Blackjack.Application.Games;

public sealed record GameView(
    Guid Id,
    int Balance,
    RoundPhase? Phase,
    DealerView? Dealer,
    IReadOnlyList<PlayerHandView> PlayerHands,
    int InsuranceBet,
    int? InsuranceNetChips,
    int? RoundNetChips,
    IReadOnlyList<GameAction> AvailableActions,
    TableRulesView Rules,
    int CardsRemaining,
    int BankrollResets);

public sealed record CardView(Rank? Rank, Suit? Suit, bool IsFaceDown)
{
    public static readonly CardView FaceDown = new(null, null, true);

    public static CardView FaceUp(Card card)
    {
        return new CardView(card.Rank, card.Suit, false);
    }
}

public sealed record DealerView(IReadOnlyList<CardView> Cards, int Total, bool IsSoft, bool IsHoleCardRevealed);

public sealed record PlayerHandView(
    IReadOnlyList<CardView> Cards,
    int Total,
    bool IsSoft,
    int Bet,
    bool IsActive,
    bool IsDoubled,
    bool IsFromSplit,
    bool IsBlackjack,
    bool IsBust,
    HandOutcome? Outcome,
    int? NetChips);

public sealed record TableRulesView(
    int MinimumBet,
    int MaximumBet,
    int BetIncrement,
    bool DealerHitsSoft17,
    string BlackjackPays);