using Blackjack.Domain.Exceptions;
using Blackjack.Domain.Rounds;
using Blackjack.Domain.Tests.TestSupport;
using static Blackjack.Domain.Cards.Rank;

namespace Blackjack.Domain.Tests.Rounds;

public class InsuranceTests
{
    [Fact]
    public void Insurance_WhenDealerHasANatural_PaysTwoToOne()
    {
        // Player 10,9. Dealer A up, K hole.
        var game = TestGame.Dealing(Ten, Ace, Nine, King);
        game.PlaceBet(TestGame.Bet);

        game.TakeInsurance();
        var round = game.CurrentRound!;

        // The main bet loses 10, and the 5-chip insurance bet wins 10, so the round breaks even.
        Assert.Equal(RoundPhase.Complete, round.Phase);
        Assert.Equal(10, round.InsuranceNetChips);
        Assert.Equal(0, round.NetChips);
        Assert.Equal(1000, game.Bankroll.Balance.Amount);
    }

    [Fact]
    public void Insurance_WhenDealerHasNoNatural_IsLostAndPlayContinues()
    {
        var game = TestGame.Dealing(Ten, Ace, Nine, Six);
        game.PlaceBet(TestGame.Bet);

        game.TakeInsurance();

        Assert.Equal(RoundPhase.PlayerTurn, game.CurrentRound!.Phase);
        Assert.Equal(-5, game.CurrentRound.InsuranceNetChips);
        Assert.Equal(985, game.Bankroll.Balance.Amount);
    }

    [Fact]
    public void DecliningInsurance_WhenDealerHasANatural_LosesOnlyTheMainBet()
    {
        var game = TestGame.Dealing(Ten, Ace, Nine, King);
        game.PlaceBet(TestGame.Bet);

        game.DeclineInsurance();

        Assert.Null(game.CurrentRound!.InsuranceNetChips);
        Assert.Equal(990, game.Bankroll.Balance.Amount);
    }

    [Fact]
    public void Insurance_WhenDealerDoesNotShowAnAce_IsRejected()
    {
        var game = TestGame.Dealing(Ten, Seven, Nine, Six);
        game.PlaceBet(TestGame.Bet);

        Assert.Throws<InvalidGameActionException>(game.TakeInsurance);
    }

    [Fact]
    public void PlayerActions_WhileInsuranceIsOffered_AreRejected()
    {
        var game = TestGame.Dealing(Ten, Ace, Nine, Six);
        game.PlaceBet(TestGame.Bet);

        Assert.Throws<InvalidGameActionException>(game.Hit);
    }
}