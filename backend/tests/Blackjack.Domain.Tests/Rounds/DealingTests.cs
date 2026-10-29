using Blackjack.Domain.Rounds;
using Blackjack.Domain.Rules;
using Blackjack.Domain.Tests.TestSupport;
using static Blackjack.Domain.Cards.Rank;

namespace Blackjack.Domain.Tests.Rounds;

public class DealingTests
{
    [Fact]
    public void InitialDeal_AlternatesPlayerAndDealer_WithTheDealersSecondCardFaceDown()
    {
        var game = TestGame.Dealing(Two, Three, Four, Five);

        game.PlaceBet(TestGame.Bet);
        var round = game.CurrentRound!;

        Assert.Equal([Two, Four], round.PlayerHands[0].Cards.Select(card => card.Rank));
        Assert.Equal([Three, Five], round.DealerHand.Cards.Select(card => card.Rank));
        Assert.Equal(Three, round.DealerHand.UpCard.Rank);
        Assert.False(round.DealerHand.IsHoleCardRevealed);
        Assert.Equal(3, round.DealerHand.VisibleValue.Total);
    }

    [Fact]
    public void PlacingABet_TakesItFromTheBankroll()
    {
        var game = TestGame.Dealing(Two, Three, Four, Five);

        game.PlaceBet(TestGame.Bet);

        Assert.Equal(990, game.Bankroll.Balance.Amount);
        Assert.Equal(RoundPhase.PlayerTurn, game.CurrentRound!.Phase);
    }

    [Fact]
    public void Shoe_IsReshuffledBeforeTheNextRound_OnceTheCutCardIsReached()
    {
        // A tiny penetration puts the cut card after the third card, so the next bet reshuffles.
        var rules = RuleSet.Bicycle with { Penetration = 0.01 };
        var game = TestGame.Dealing(rules, Ten, Seven, Nine, Eight, Two);
        game.PlaceBet(TestGame.Bet);
        game.Stand();

        game.PlaceBet(TestGame.Bet);

        // The stacked shuffler puts the same cards back on top, so a reshuffle deals them again.
        Assert.Equal([Ten, Nine], game.CurrentRound!.PlayerHands[0].Cards.Select(card => card.Rank));
    }
}