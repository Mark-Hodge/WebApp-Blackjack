using Blackjack.Domain.Cards;
using Blackjack.Domain.Tests.TestSupport;
using static Blackjack.Domain.Cards.Rank;

namespace Blackjack.Domain.Tests.Hands;

public class HandValueTests
{
    [Theory]
    [InlineData(new[] { Ten, Six }, 16, false)]
    [InlineData(new[] { King, Queen }, 20, false)]
    [InlineData(new[] { Ace, Six }, 17, true)]         // ace counts as 11
    [InlineData(new[] { Ace, Six, Ten }, 17, false)]   // 11 would bust, so the ace drops to 1
    [InlineData(new[] { Ace, Ace }, 12, true)]         // only one ace can be 11
    [InlineData(new[] { Ace, Ace, Nine }, 21, true)]
    [InlineData(new[] { Ace, Ace, Ace, Ace, Seven }, 21, true)]
    [InlineData(new[] { Ace, Five, Five, Ten }, 21, false)]
    [InlineData(new[] { King, Queen, Two }, 22, false)]
    public void Total_CountsAcesAsOneOrElevenWhicheverIsBetter(Rank[] ranks, int expectedTotal, bool expectedSoft)
    {
        var hand = TestGame.PlayerHand(ranks);

        Assert.Equal(expectedTotal, hand.Total);
        Assert.Equal(expectedSoft, hand.IsSoft);
    }

    [Fact]
    public void Hand_Over21_IsBust()
    {
        Assert.True(TestGame.PlayerHand(King, Queen, Two).IsBust);
        Assert.False(TestGame.PlayerHand(King, Queen, Ace).IsBust);
    }

    [Theory]
    [InlineData(Ace, King)]
    [InlineData(Ten, Ace)]
    [InlineData(Ace, Jack)]
    public void AceAndTenCard_AsFirstTwoCards_IsANatural(Rank first, Rank second)
    {
        Assert.True(TestGame.PlayerHand(first, second).IsNatural);
    }

    [Fact]
    public void TwentyOneWithThreeCards_IsNotANatural()
    {
        Assert.False(TestGame.PlayerHand(Ace, Five, Five).IsNatural);
    }

    [Fact]
    public void AceAndTenCard_AfterASplit_IsNotANatural()
    {
        var hand = TestGame.PlayerHand(isFromSplit: true, Ace, King);

        Assert.Equal(21, hand.Total);
        Assert.False(hand.IsNatural);
    }

    [Theory]
    [InlineData(Eight, Eight, true)]
    [InlineData(King, King, true)]
    [InlineData(King, Queen, false)] // Bicycle: same denomination, not just the same value
    public void IsPair_RequiresTheSameRank(Rank first, Rank second, bool expected)
    {
        Assert.Equal(expected, TestGame.PlayerHand(first, second).IsPair);
    }
}