using Blackjack.Domain.Betting;
using Blackjack.Domain.Exceptions;
using Blackjack.Domain.Rounds;
using Blackjack.Domain.Rules;
using Blackjack.Domain.Tests.TestSupport;
using static Blackjack.Domain.Cards.Rank;

namespace Blackjack.Domain.Tests.Betting;

public class BettingTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(5)]      // below the minimum
    [InlineData(510)]    // above the maximum
    [InlineData(15)]     // not a multiple of 10
    public void Bet_OutsideTheTableLimits_IsRejected(int amount)
    {
        var game = TestGame.Dealing(Ten, Seven, Nine, Ten);

        Assert.Throws<InvalidBetException>(() => game.PlaceBet(new Chips(amount)));
        Assert.Equal(1000, game.Bankroll.Balance.Amount);
    }

    [Fact]
    public void Bet_LargerThanTheBankroll_IsRejected()
    {
        var game = TestGame.Dealing(RuleSet.Bicycle with { StartingBankroll = new(10) }, Ten, Seven, Nine, Ten);

        Assert.Throws<InsufficientChipsException>(() => game.PlaceBet(new Chips(20)));
    }

    [Fact]
    public void ResetBankroll_IsOfferedOnlyWhenThePlayerCannotCoverTheMinimumBet()
    {
        // Lose the only 10 chips: player 16 stands, dealer 6,9 draws a 4 for 19.
        var rules = RuleSet.Bicycle with { StartingBankroll = new(10) };
        var game = TestGame.Dealing(rules, Ten, Six, Six, Nine, Four);
        Assert.Throws<InvalidGameActionException>(game.ResetBankroll);

        game.PlaceBet(TestGame.Bet);
        game.Stand();
        Assert.Equal([GameAction.ResetBankroll], game.GetAvailableActions());

        game.ResetBankroll();

        Assert.Equal(10, game.Bankroll.Balance.Amount);
        Assert.Equal(1, game.BankrollResets);
    }

    [Fact]
    public void Chips_CannotBeNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Chips(-1));
    }

    [Fact]
    public void Chips_MultiplyByARatio_RoundsDown()
    {
        Assert.Equal(7, new Chips(5).MultiplyBy(3, 2).Amount);
    }
}