using Blackjack.Domain.Rounds;
using Blackjack.Domain.Tests.TestSupport;
using static Blackjack.Domain.Cards.Rank;

namespace Blackjack.Domain.Tests.Rounds;

public class NaturalsTests
{
    [Fact]
    public void PlayerNatural_IsPaidThreeToTwoImmediately()
    {
        // Player A,K. Dealer 9 up, 7 hole: no peek, so the natural is paid straight away.
        var game = TestGame.Dealing(Ace, Nine, King, Seven);

        game.PlaceBet(TestGame.Bet);
        var round = game.CurrentRound!;

        Assert.Equal(RoundPhase.Complete, round.Phase);
        Assert.Equal(HandOutcome.Win, round.PlayerHands[0].Result!.Outcome);
        Assert.Equal(15, round.NetChips);
        Assert.Equal(1015, game.Bankroll.Balance.Amount);
        Assert.Equal(2, round.DealerHand.Cards.Count); // dealer doesn't draw
    }

    [Fact]
    public void PlayerAndDealerNaturals_AreAStandOff()
    {
        // Player A,K. Dealer K up, A hole: the dealer peeks and finds a natural.
        var game = TestGame.Dealing(Ace, King, King, Ace);

        game.PlaceBet(TestGame.Bet);

        Assert.Equal(HandOutcome.Push, game.CurrentRound!.PlayerHands[0].Result!.Outcome);
        Assert.Equal(1000, game.Bankroll.Balance.Amount);
    }

    [Fact]
    public void DealerNatural_WithTenCardShowing_EndsTheRoundBeforeThePlayerActs()
    {
        // Player 10,9. Dealer K up, A hole.
        var game = TestGame.Dealing(Ten, King, Nine, Ace);

        game.PlaceBet(TestGame.Bet);
        var round = game.CurrentRound!;

        Assert.Equal(RoundPhase.Complete, round.Phase);
        Assert.True(round.DealerHand.IsHoleCardRevealed);
        Assert.Equal(HandOutcome.Loss, round.PlayerHands[0].Result!.Outcome);
        Assert.Equal(990, game.Bankroll.Balance.Amount);
    }

    [Fact]
    public void DealerTenCardShowing_WithoutANatural_PlayContinues()
    {
        var game = TestGame.Dealing(Ten, King, Nine, Seven);

        game.PlaceBet(TestGame.Bet);

        Assert.Equal(RoundPhase.PlayerTurn, game.CurrentRound!.Phase);
        Assert.False(game.CurrentRound.DealerHand.IsHoleCardRevealed);
    }

    [Fact]
    public void DealerAceShowing_OffersInsuranceBeforeThePeek()
    {
        var game = TestGame.Dealing(Ten, Ace, Nine, King);

        game.PlaceBet(TestGame.Bet);

        Assert.Equal(RoundPhase.InsuranceOffered, game.CurrentRound!.Phase);
        Assert.False(game.CurrentRound.DealerHand.IsHoleCardRevealed);
        Assert.Equal([GameAction.TakeInsurance, GameAction.DeclineInsurance], game.GetAvailableActions());
    }
}