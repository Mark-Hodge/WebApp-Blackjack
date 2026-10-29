using Blackjack.Domain.Rules;
using Blackjack.Domain.Tests.TestSupport;
using static Blackjack.Domain.Cards.Rank;

namespace Blackjack.Domain.Tests.Rounds;

public class DealerPlayTests
{
    [Fact]
    public void Dealer_HitsSixteen_AndStandsOnHardSeventeen()
    {
        // Dealer 6,10 = 16 draws an ace: 11 would bust, so it counts as 1 for a hard 17.
        var game = TestGame.Dealing(Ten, Six, Nine, Ten, Ace);
        game.PlaceBet(TestGame.Bet);

        game.Stand();
        var dealer = game.CurrentRound!.DealerHand;

        Assert.Equal(3, dealer.Cards.Count);
        Assert.Equal(17, dealer.Total);
        Assert.False(dealer.IsSoft);
    }

    [Fact]
    public void Dealer_StandsOnSoftSeventeen_UnderBicycleRules()
    {
        // Dealer 6 up, A hole = soft 17.
        var game = TestGame.Dealing(Ten, Six, Nine, Ace);
        game.PlaceBet(TestGame.Bet);

        game.Stand();
        var dealer = game.CurrentRound!.DealerHand;

        Assert.Equal(2, dealer.Cards.Count);
        Assert.Equal(17, dealer.Total);
        Assert.True(dealer.IsSoft);
    }

    [Fact]
    public void Dealer_HitsSoftSeventeen_WhenTheRuleSetSaysSo()
    {
        var rules = RuleSet.Bicycle with { DealerHitsSoft17 = true };
        var game = TestGame.Dealing(rules, Ten, Six, Nine, Ace, Two);
        game.PlaceBet(TestGame.Bet);

        game.Stand();

        Assert.Equal(19, game.CurrentRound!.DealerHand.Total);
    }

    [Fact]
    public void Dealer_Busting_PaysEveryLiveHand()
    {
        // Player 18 stands. Dealer 6,10 draws a K and busts.
        var game = TestGame.Dealing(Ten, Six, Eight, Ten, King);
        game.PlaceBet(TestGame.Bet);

        game.Stand();

        Assert.True(game.CurrentRound!.DealerHand.IsBust);
        Assert.Equal(1010, game.Bankroll.Balance.Amount);
    }
}