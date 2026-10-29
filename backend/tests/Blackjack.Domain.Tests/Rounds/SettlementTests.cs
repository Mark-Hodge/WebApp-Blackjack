using Blackjack.Domain.Cards;
using Blackjack.Domain.Rounds;
using Blackjack.Domain.Rules;
using Blackjack.Domain.Tests.TestSupport;
using static Blackjack.Domain.Cards.Rank;

namespace Blackjack.Domain.Tests.Rounds;

public class SettlementTests
{
    [Theory]
    [InlineData(new[] { Ten, Nine }, new[] { Ten, Eight }, HandOutcome.Win)]
    [InlineData(new[] { Ten, Eight }, new[] { Ten, Nine }, HandOutcome.Loss)]
    [InlineData(new[] { Ten, Eight }, new[] { Nine, Nine }, HandOutcome.Push)]
    [InlineData(new[] { Ten, Seven }, new[] { Ten, Six, Nine }, HandOutcome.Win)]        // dealer busts
    [InlineData(new[] { Ten, Six, Ten }, new[] { Ten, Six, Nine }, HandOutcome.Loss)]    // player bust loses even if dealer busts
    [InlineData(new[] { Ace, King }, new[] { Ten, Five, Six }, HandOutcome.Win)]         // natural beats a three-card 21
    [InlineData(new[] { Ten, Five, Six }, new[] { Ace, King }, HandOutcome.Loss)]
    [InlineData(new[] { Ace, King }, new[] { Ace, Queen }, HandOutcome.Push)]
    public void DetermineOutcome_FollowsBicycleWinConditions(Rank[] player, Rank[] dealer, HandOutcome expected)
    {
        var outcome = Settlement.DetermineOutcome(TestGame.PlayerHand(player), TestGame.DealerHand(dealer));

        Assert.Equal(expected, outcome);
    }

    [Theory]
    [InlineData(new[] { Ace, King }, new[] { Ten, Eight }, 25)]   // natural: stake + 3:2
    [InlineData(new[] { Ten, Nine }, new[] { Ten, Eight }, 20)]   // win: stake + 1:1
    [InlineData(new[] { Ten, Eight }, new[] { Nine, Nine }, 10)]  // push: stake back
    [InlineData(new[] { Ten, Seven }, new[] { Ten, Nine }, 0)]    // loss
    public void Settle_PaysTheRightAmountOnATenChipBet(Rank[] player, Rank[] dealer, int expectedPayout)
    {
        var result = Settlement.Settle(TestGame.PlayerHand(player), TestGame.DealerHand(dealer), RuleSet.Bicycle);

        Assert.Equal(expectedPayout, result.Payout.Amount);
    }
}