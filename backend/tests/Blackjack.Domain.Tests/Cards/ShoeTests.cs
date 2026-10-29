using Blackjack.Domain.Cards;
using Blackjack.Domain.Tests.TestSupport;

namespace Blackjack.Domain.Tests.Cards;

public class ShoeTests
{
    private static Shoe NewSixDeckShoe() => new(deckCount: 6, penetration: 0.75, new StackedShuffler());

    [Fact]
    public void NewShoe_HoldsSixCompleteDecks()
    {
        var shoe = NewSixDeckShoe();

        var dealt = Enumerable.Range(0, 312).Select(_ => shoe.Draw()).ToList();

        Assert.Equal(52, dealt.Distinct().Count());
        Assert.All(dealt.GroupBy(card => card), copies => Assert.Equal(6, copies.Count()));
    }

    [Fact]
    public void CutCard_IsReachedAfterThreeQuartersOfTheShoe()
    {
        var shoe = NewSixDeckShoe();

        for (var i = 0; i < 233; i++) shoe.Draw();
        Assert.False(shoe.IsCutCardReached);

        shoe.Draw(); // card 234 = 312 * 0.75
        Assert.True(shoe.IsCutCardReached);
    }

    [Fact]
    public void Draw_FromAnEmptyShoe_ReshufflesInsteadOfFailing()
    {
        var shoe = NewSixDeckShoe();
        for (var i = 0; i < 312; i++) shoe.Draw();

        shoe.Draw();

        Assert.Equal(311, shoe.CardsRemaining);
    }
}