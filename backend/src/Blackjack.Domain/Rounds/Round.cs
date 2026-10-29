using Blackjack.Domain.Betting;
using Blackjack.Domain.Cards;
using Blackjack.Domain.Exceptions;
using Blackjack.Domain.Hands;
using Blackjack.Domain.Rules;

namespace Blackjack.Domain.Rounds;

public sealed class Round
{
    private readonly Shoe _shoe;
    private readonly Bankroll _bankroll;
    private readonly RuleSet _rules;
    private readonly List<PlayerHand> _playerHands = [];

    private Round(Shoe shoe, Bankroll bankroll, RuleSet rules)
    {
        _shoe = shoe;
        _bankroll = bankroll;
        _rules = rules;
    }

    public RoundPhase Phase { get; private set; }

    public DealerHand DealerHand { get; } = new();

    public IReadOnlyList<PlayerHand> PlayerHands => _playerHands;

    public int ActiveHandIndex { get; private set; }

    public Chips InsuranceBet { get; private set; }

    public int? InsuranceNetChips { get; private set; }

    public int? NetChips => Phase == RoundPhase.Complete
        ? _playerHands.Sum(hand => hand.Result!.NetChips) + (InsuranceNetChips ?? 0)
        : null;

    private Chips InsuranceCost => _playerHands[0].Bet.Half();

    internal static Round Start(Chips bet, Shoe shoe, Bankroll bankroll, RuleSet rules)
    {
        rules.ValidateBet(bet);
        bankroll.Debit(bet);

        var round = new Round(shoe, bankroll, rules);
        round._playerHands.Add(new PlayerHand(bet));
        round.DealInitialCards();

        if (round.DealerHand.UpCard.IsAce)
            round.Phase = RoundPhase.InsuranceOffered;
        else
            round.CheckForNaturals();

        return round;
    }

    public IReadOnlyList<GameAction> GetAvailableActions()
    {
        switch (Phase)
        {
            case RoundPhase.InsuranceOffered:
                return _bankroll.CanCover(InsuranceCost)
                    ? [GameAction.TakeInsurance, GameAction.DeclineInsurance]
                    : [GameAction.DeclineInsurance];

            case RoundPhase.PlayerTurn:
                var hand = _playerHands[ActiveHandIndex];
                var actions = new List<GameAction> { GameAction.Hit, GameAction.Stand };
                if (IsDoubleDownAllowed(hand) && _bankroll.CanCover(hand.Bet)) actions.Add(GameAction.DoubleDown);
                if (IsSplitAllowed(hand) && _bankroll.CanCover(hand.Bet)) actions.Add(GameAction.Split);
                return actions;

            default:
                return [];
        }
    }

    // ---- Insurance --------------------------------------------------------------------------

    internal void TakeInsurance()
    {
        RequirePhase(RoundPhase.InsuranceOffered, "Insurance is only offered when the dealer shows an ace.");
        _bankroll.Debit(InsuranceCost);
        InsuranceBet = InsuranceCost;
        CheckForNaturals();
    }

    internal void DeclineInsurance()
    {
        RequirePhase(RoundPhase.InsuranceOffered, "Insurance is only offered when the dealer shows an ace.");
        CheckForNaturals();
    }

    // ---- Player actions ---------------------------------------------------------------------

    internal void Hit()
    {
        var hand = RequireActiveHand();
        hand.Add(_shoe.Draw());

        if (hand.IsBust || hand.Total == 21) hand.Complete();

        MoveToNextHandIfComplete();
    }

    internal void Stand()
    {
        RequireActiveHand().Complete();
        MoveToNextHandIfComplete();
    }

    internal void DoubleDown()
    {
        var hand = RequireActiveHand();
        if (!IsDoubleDownAllowed(hand))
            throw new InvalidGameActionException(
                $"You can only double down on your first two cards when they total {string.Join(", ", _rules.DoubleDownTotals.Order())}.");

        _bankroll.Debit(hand.Bet);
        hand.DoubleBet();
        hand.Add(_shoe.Draw());
        hand.Complete();
        MoveToNextHandIfComplete();
    }

    internal void Split()
    {
        var hand = RequireActiveHand();
        if (!IsSplitAllowed(hand))
            throw new InvalidGameActionException("You can only split a pair, and only once per round.");

        _bankroll.Debit(hand.Bet);
        var secondHand = hand.Split();
        _playerHands.Insert(ActiveHandIndex + 1, secondHand);

        DealSecondCard(hand);
        if (hand.IsSplitAces)
        {
            DealSecondCard(secondHand);
            hand.Complete();
            secondHand.Complete();
        }

        MoveToNextHandIfComplete();
    }

    // ---- Round flow -------------------------------------------------------------------------
    private void DealInitialCards()
    {
        foreach (var hand in _playerHands) hand.Add(_shoe.Draw());
        DealerHand.Add(_shoe.Draw());
        foreach (var hand in _playerHands) hand.Add(_shoe.Draw());
        DealerHand.Add(_shoe.Draw());
    }

    private void CheckForNaturals()
    {
        var dealerHasNatural = DealerHand.ShouldPeekForNatural && DealerHand.IsNatural;
        SettleInsurance(dealerHasNatural);

        if (dealerHasNatural || _playerHands[0].IsNatural)
        {
            FinishRound();
            return;
        }

        Phase = RoundPhase.PlayerTurn;
    }

    private void SettleInsurance(bool dealerHasNatural)
    {
        if (InsuranceBet == Chips.Zero) return;

        if (dealerHasNatural)
        {
            var winnings = InsuranceBet * _rules.InsurancePayoutRatio;
            _bankroll.Credit(InsuranceBet + winnings);
            InsuranceNetChips = winnings.Amount;
        }
        else
        {
            InsuranceNetChips = -InsuranceBet.Amount;
        }
    }

    private void MoveToNextHandIfComplete()
    {
        while (_playerHands[ActiveHandIndex].IsComplete)
        {
            ActiveHandIndex++;
            if (ActiveHandIndex == _playerHands.Count)
            {
                FinishRound();
                return;
            }

            var nextHand = _playerHands[ActiveHandIndex];
            if (nextHand.Cards.Count == 1) DealSecondCard(nextHand);
        }
    }

    private void DealSecondCard(PlayerHand hand)
    {
        hand.Add(_shoe.Draw());
        if (hand.Total == 21) hand.Complete();
    }

    private void FinishRound()
    {
        DealerHand.RevealHoleCard();

        if (_playerHands.Any(hand => !hand.IsBust && !hand.IsNatural))
            while (DealerHand.MustHit(_rules))
                DealerHand.Add(_shoe.Draw());

        foreach (var hand in _playerHands)
        {
            var result = Settlement.Settle(hand, DealerHand, _rules);
            _bankroll.Credit(result.Payout);
            hand.Settle(result);
        }

        Phase = RoundPhase.Complete;
    }

    // ---- Rule checks ------------------------------------------------------------------------
    private bool IsDoubleDownAllowed(PlayerHand hand)
    {
        return hand.Cards.Count == 2
               && _rules.DoubleDownTotals.Contains(hand.Total)
               && !hand.IsSplitAces
               && (!hand.IsFromSplit || _rules.AllowDoubleAfterSplit);
    }

    private bool IsSplitAllowed(PlayerHand hand)
    {
        return hand.IsPair && _playerHands.Count == 1;
    }

    private PlayerHand RequireActiveHand()
    {
        RequirePhase(RoundPhase.PlayerTurn, "It isn't your turn to play a hand.");
        return _playerHands[ActiveHandIndex];
    }

    private void RequirePhase(RoundPhase expected, string message)
    {
        if (Phase != expected) throw new InvalidGameActionException(message);
    }
}