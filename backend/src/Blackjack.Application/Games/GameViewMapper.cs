using Blackjack.Domain.Hands;
using Blackjack.Domain.Rounds;
using Blackjack.Domain.Rules;
using Blackjack.Domain.Sessions;

namespace Blackjack.Application.Games;

internal static class GameViewMapper
{
    public static GameView ToView(GameSession session)
    {
        var round = session.CurrentRound;

        return new GameView(
            session.Id,
            session.Bankroll.Balance.Amount,
            round?.Phase,
            round is null ? null : ToView(round.DealerHand),
            round is null ? [] : ToPlayerHandViews(round),
            round?.InsuranceBet.Amount ?? 0,
            round?.InsuranceNetChips,
            round?.NetChips,
            session.GetAvailableActions(),
            ToView(session.Rules),
            session.CardsRemainingInShoe,
            session.BankrollResets);
    }

    private static DealerView ToView(DealerHand dealer)
    {
        var cards = dealer.Cards
            .Select((card, index) =>
                index == 1 && !dealer.IsHoleCardRevealed ? CardView.FaceDown : CardView.FaceUp(card))
            .ToList();
        var visible = dealer.VisibleValue;

        return new DealerView(cards, visible.Total, visible.IsSoft, dealer.IsHoleCardRevealed);
    }

    private static List<PlayerHandView> ToPlayerHandViews(Round round)
    {
        return round.PlayerHands
            .Select((hand, index) => new PlayerHandView(
                hand.Cards.Select(CardView.FaceUp).ToList(),
                hand.Total,
                hand.IsSoft,
                hand.Bet.Amount,
                round.Phase == RoundPhase.PlayerTurn && index == round.ActiveHandIndex,
                hand.IsDoubled,
                hand.IsFromSplit,
                hand.IsNatural,
                hand.IsBust,
                hand.Result?.Outcome,
                hand.Result?.NetChips))
            .ToList();
    }

    private static TableRulesView ToView(RuleSet rules)
    {
        return new TableRulesView(
            rules.MinimumBet.Amount,
            rules.MaximumBet.Amount,
            rules.BetIncrement,
            rules.DealerHitsSoft17,
            $"{rules.BlackjackPayoutNumerator}:{rules.BlackjackPayoutDenominator}");
    }
}