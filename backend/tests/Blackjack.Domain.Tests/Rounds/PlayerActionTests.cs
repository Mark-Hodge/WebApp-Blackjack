using Blackjack.Domain.Cards;
using Blackjack.Domain.Exceptions;
using Blackjack.Domain.Rounds;
using Blackjack.Domain.Rules;
using Blackjack.Domain.Tests.TestSupport;
using static Blackjack.Domain.Cards.Rank;

namespace Blackjack.Domain.Tests.Rounds;

public class PlayerActionTests
{
    // ---- Hit and stand ----------------------------------------------------------------------
    [Fact]
    public void Hit_AddsACard_AndLeavesThePlayerToAct()
    {
        var game = TestGame.Dealing(Ten, Seven, Two, Nine, Three);
        game.PlaceBet(TestGame.Bet);

        game.Hit();

        Assert.Equal(15, game.CurrentRound!.PlayerHands[0].Total);
        Assert.Equal(RoundPhase.PlayerTurn, game.CurrentRound.Phase);
    }

    [Fact]
    public void Hit_ToABust_LosesAtOnce_AndTheDealerDoesNotDraw()
    {
        // Player 10,6 hits a king. Dealer 6 up, 10 hole: 16, which would have to hit.
        var game = TestGame.Dealing(Ten, Six, Six, Ten, King);
        game.PlaceBet(TestGame.Bet);

        game.Hit();
        var round = game.CurrentRound!;

        Assert.Equal(RoundPhase.Complete, round.Phase);
        Assert.Equal(HandOutcome.Loss, round.PlayerHands[0].Result!.Outcome);
        Assert.Equal(2, round.DealerHand.Cards.Count);
        Assert.Equal(990, game.Bankroll.Balance.Amount);
    }

    [Fact]
    public void Hit_To21_StandsAutomatically()
    {
        // Player 10,5 hits a 6 for 21. Dealer 7,10 stands on 17.
        var game = TestGame.Dealing(Ten, Seven, Five, Ten, Six);
        game.PlaceBet(TestGame.Bet);

        game.Hit();

        Assert.Equal(RoundPhase.Complete, game.CurrentRound!.Phase);
        Assert.Equal(1010, game.Bankroll.Balance.Amount);
    }

    [Fact]
    public void Stand_HandsPlayToTheDealer()
    {
        // Player 19 stands. Dealer 10,6 draws a 5 for 21.
        var game = TestGame.Dealing(Ten, Ten, Nine, Six, Five);
        game.PlaceBet(TestGame.Bet);

        game.Stand();
        var round = game.CurrentRound!;

        Assert.Equal(21, round.DealerHand.Total);
        Assert.Equal(HandOutcome.Loss, round.PlayerHands[0].Result!.Outcome);
    }

    // ---- Double down ------------------------------------------------------------------------
    [Theory]
    [InlineData(Five, Four)]  // 9
    [InlineData(Six, Four)]   // 10
    [InlineData(Six, Five)]   // 11
    public void DoubleDown_OnNineTenOrEleven_DoublesTheBetAndDealsExactlyOneCard(Rank first, Rank second)
    {
        var game = TestGame.Dealing(first, Seven, second, Ten, Two);
        game.PlaceBet(TestGame.Bet);

        game.DoubleDown();
        var hand = game.CurrentRound!.PlayerHands[0];

        Assert.True(hand.IsDoubled);
        Assert.Equal(20, hand.Bet.Amount);
        Assert.Equal(3, hand.Cards.Count);
        Assert.Equal(RoundPhase.Complete, game.CurrentRound.Phase);
    }

    [Fact]
    public void DoubleDown_Win_PaysOnTheDoubledBet()
    {
        // Player 6,5 = 11 doubles and draws a 10. Dealer 7,10 stands on 17.
        var game = TestGame.Dealing(Six, Seven, Five, Ten, Ten);
        game.PlaceBet(TestGame.Bet);

        game.DoubleDown();

        Assert.Equal(1020, game.Bankroll.Balance.Amount);
    }

    [Fact]
    public void DoubleDown_OnTwelve_IsNotAllowed()
    {
        var game = TestGame.Dealing(Ten, Seven, Two, Ten);
        game.PlaceBet(TestGame.Bet);

        Assert.DoesNotContain(GameAction.DoubleDown, game.GetAvailableActions());
        Assert.Throws<InvalidGameActionException>(game.DoubleDown);
    }

    [Fact]
    public void DoubleDown_WithoutChipsToCoverIt_IsNotOffered()
    {
        var rules = RuleSet.Bicycle with { StartingBankroll = new(10) };
        var game = TestGame.Dealing(rules, Six, Seven, Five, Ten);
        game.PlaceBet(TestGame.Bet);

        Assert.DoesNotContain(GameAction.DoubleDown, game.GetAvailableActions());
        Assert.Throws<InsufficientChipsException>(game.DoubleDown);
    }

    // ---- Split ------------------------------------------------------------------------------
    [Fact]
    public void Split_PlaysTwoHandsLeftToRight_EachWithItsOwnBet()
    {
        // Player 8,8 splits. The first hand gets a 3; the second gets a 2 once the first stands.
        var game = TestGame.Dealing(Eight, Seven, Eight, Ten, Three, Two);
        game.PlaceBet(TestGame.Bet);

        game.Split();
        var round = game.CurrentRound!;
        Assert.Equal(980, game.Bankroll.Balance.Amount);
        Assert.Equal([Eight, Three], round.PlayerHands[0].Cards.Select(card => card.Rank));
        Assert.Single(round.PlayerHands[1].Cards);
        Assert.Equal(0, round.ActiveHandIndex);

        game.Stand();

        Assert.Equal(1, round.ActiveHandIndex);
        Assert.Equal([Eight, Two], round.PlayerHands[1].Cards.Select(card => card.Rank));
        Assert.All(round.PlayerHands, hand => Assert.Equal(10, hand.Bet.Amount));
    }

    [Fact]
    public void SplitAces_GetOneCardEach_AndAceWithTenCardPaysOneToOne()
    {
        // Player A,A splits: first ace gets a K (21, not a natural), second gets a 9 (20). Dealer 7,10 = 17.
        var game = TestGame.Dealing(Ace, Seven, Ace, Ten, King, Nine);
        game.PlaceBet(TestGame.Bet);

        game.Split();
        var round = game.CurrentRound!;

        Assert.Equal(RoundPhase.Complete, round.Phase);
        Assert.False(round.PlayerHands[0].IsNatural);
        Assert.All(round.PlayerHands, hand => Assert.Equal(10, hand.Result!.NetChips));
        Assert.Equal(1020, game.Bankroll.Balance.Amount);
    }

    [Fact]
    public void Split_CanOnlyHappenOncePerRound()
    {
        var game = TestGame.Dealing(Eight, Seven, Eight, Ten, Eight);
        game.PlaceBet(TestGame.Bet);
        game.Split(); // the first hand is now 8,8 again

        Assert.DoesNotContain(GameAction.Split, game.GetAvailableActions());
        Assert.Throws<InvalidGameActionException>(game.Split);
    }

    [Fact]
    public void Split_OnTwoDifferentTenCards_IsNotAllowed()
    {
        var game = TestGame.Dealing(King, Seven, Queen, Ten);
        game.PlaceBet(TestGame.Bet);

        Assert.Throws<InvalidGameActionException>(game.Split);
    }

    // ---- Out of turn ------------------------------------------------------------------------
    [Fact]
    public void Actions_BeforeABetIsPlaced_AreRejected()
    {
        var game = TestGame.Dealing(Ten, Seven, Nine, Ten);

        Assert.Throws<InvalidGameActionException>(game.Hit);
        Assert.Equal([GameAction.PlaceBet], game.GetAvailableActions());
    }

    [Fact]
    public void PlacingABet_DuringARound_IsRejected()
    {
        var game = TestGame.Dealing(Ten, Seven, Nine, Ten);
        game.PlaceBet(TestGame.Bet);

        Assert.Throws<InvalidGameActionException>(() => game.PlaceBet(TestGame.Bet));
    }
}