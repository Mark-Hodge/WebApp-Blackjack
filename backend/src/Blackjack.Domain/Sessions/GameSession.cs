using Blackjack.Domain.Abstractions;
using Blackjack.Domain.Betting;
using Blackjack.Domain.Cards;
using Blackjack.Domain.Exceptions;
using Blackjack.Domain.Rounds;
using Blackjack.Domain.Rules;

namespace Blackjack.Domain.Sessions;

public sealed class GameSession
{
    private readonly Shoe _shoe;

    public GameSession(Guid id, RuleSet rules, IShuffler shuffler)
    {
        Id = id;
        Rules = rules;
        Bankroll = new Bankroll(rules.StartingBankroll);
        _shoe = new Shoe(rules.DeckCount, rules.Penetration, shuffler);
    }

    public Guid Id { get; }

    public RuleSet Rules { get; }

    public Bankroll Bankroll { get; }

    public Round? CurrentRound { get; private set; }

    public int BankrollResets { get; private set; }

    public int CardsRemainingInShoe => _shoe.CardsRemaining;

    public bool IsRoundInProgress => CurrentRound is not null && CurrentRound.Phase != RoundPhase.Complete;

    private bool CanResetBankroll => !IsRoundInProgress && !Bankroll.CanCover(Rules.MinimumBet);

    public IReadOnlyList<GameAction> GetAvailableActions()
    {
        if (IsRoundInProgress) return CurrentRound!.GetAvailableActions();
        if (CanResetBankroll) return [GameAction.ResetBankroll];
        return [GameAction.PlaceBet];
    }

    public void PlaceBet(Chips bet)
    {
        if (IsRoundInProgress)
            throw new InvalidGameActionException("Finish the current round before placing a new bet.");

        if (_shoe.IsCutCardReached) _shoe.Shuffle();

        CurrentRound = Round.Start(bet, _shoe, Bankroll, Rules);
    }

    public void Hit()
    {
        RequireRoundInProgress().Hit();
    }

    public void Stand()
    {
        RequireRoundInProgress().Stand();
    }

    public void DoubleDown()
    {
        RequireRoundInProgress().DoubleDown();
    }

    public void Split()
    {
        RequireRoundInProgress().Split();
    }

    public void TakeInsurance()
    {
        RequireRoundInProgress().TakeInsurance();
    }

    public void DeclineInsurance()
    {
        RequireRoundInProgress().DeclineInsurance();
    }

    public void ResetBankroll()
    {
        if (!CanResetBankroll)
            throw new InvalidGameActionException(
                "You can only reset your bankroll when you can't cover the minimum bet.");

        Bankroll.ResetTo(Rules.StartingBankroll);
        BankrollResets++;
    }

    private Round RequireRoundInProgress()
    {
        return IsRoundInProgress
            ? CurrentRound!
            : throw new InvalidGameActionException("Place a bet to start a round.");
    }
}