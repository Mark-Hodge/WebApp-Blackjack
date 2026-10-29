using Blackjack.Application.Abstractions;
using Blackjack.Application.Games;
using Blackjack.Domain.Abstractions;
using Blackjack.Domain.Cards;
using Blackjack.Domain.Exceptions;
using Blackjack.Domain.Rounds;
using Blackjack.Domain.Rules;
using Blackjack.Domain.Sessions;
using static Blackjack.Domain.Cards.Rank;

namespace Blackjack.Application.Tests.Games;

public class GameServiceTests
{
    private readonly FakeGameSessionStore _store = new();

    [Fact]
    public async Task Create_StartsANewGameReadyForABet()
    {
        var game = await NewService().CreateAsync(CancellationToken.None);

        Assert.Equal(1000, game.Balance);
        Assert.Null(game.Phase);
        Assert.Empty(game.PlayerHands);
        Assert.Equal([GameAction.PlaceBet], game.AvailableActions);
        Assert.NotNull(await _store.FindAsync(game.Id, CancellationToken.None));
    }

    [Fact]
    public async Task Get_UnknownGame_Throws()
    {
        await Assert.ThrowsAsync<GameNotFoundException>(() => NewService().GetAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task View_HidesTheHoleCardUntilTheDealerTurnsItOver()
    {
        // Player 10,9. Dealer 7 up, K hole.
        var service = NewService(Ten, Seven, Nine, King);
        var game = await service.CreateAsync(CancellationToken.None);

        var duringPlay = await service.PlaceBetAsync(game.Id, 10, CancellationToken.None);

        var hidden = duringPlay.Dealer!.Cards[1];
        Assert.True(hidden.IsFaceDown);
        Assert.Null(hidden.Rank);
        Assert.Null(hidden.Suit);
        Assert.Equal(7, duringPlay.Dealer.Total);

        var afterStand = await service.PerformAsync(game.Id, GameAction.Stand, CancellationToken.None);

        Assert.Equal(King, afterStand.Dealer!.Cards[1].Rank);
        Assert.Equal(17, afterStand.Dealer.Total);
        Assert.Equal(HandOutcome.Win, afterStand.PlayerHands[0].Outcome);
        Assert.Equal(10, afterStand.RoundNetChips);
    }

    [Fact]
    public async Task View_MarksTheHandBeingPlayed()
    {
        var service = NewService(Ten, Seven, Six, King);
        var game = await service.CreateAsync(CancellationToken.None);

        var view = await service.PlaceBetAsync(game.Id, 10, CancellationToken.None);

        Assert.True(view.PlayerHands[0].IsActive);
        Assert.Equal([GameAction.Hit, GameAction.Stand], view.AvailableActions);
    }

    [Fact]
    public async Task PlaceBet_NonPositiveAmount_IsRejected()
    {
        var service = NewService();
        var game = await service.CreateAsync(CancellationToken.None);

        await Assert.ThrowsAsync<InvalidBetException>(() => service.PlaceBetAsync(game.Id, -10, CancellationToken.None));
    }

    [Fact]
    public async Task Perform_PlaceBetAction_IsRejected()
    {
        var service = NewService();
        var game = await service.CreateAsync(CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.PerformAsync(game.Id, GameAction.PlaceBet, CancellationToken.None));
    }

    private GameService NewService(params Rank[] ranksOnTop) =>
        new(_store, new StackedShuffler(ranksOnTop), RuleSet.Bicycle);

    private sealed class FakeGameSessionStore : IGameSessionStore
    {
        private readonly Dictionary<Guid, GameSession> _sessions = [];

        public Task<GameSession?> FindAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(_sessions.GetValueOrDefault(id));

        public Task SaveAsync(GameSession session, CancellationToken cancellationToken)
        {
            _sessions[session.Id] = session;
            return Task.CompletedTask;
        }
    }

    private sealed class StackedShuffler(params Rank[] ranksOnTop) : IShuffler
    {
        public void Shuffle(IList<Card> cards)
        {
            var suits = Enum.GetValues<Suit>();
            for (var position = 0; position < ranksOnTop.Length; position++)
            {
                var wanted = new Card(ranksOnTop[position], suits[position % suits.Length]);
                var source = Enumerable.Range(position, cards.Count - position).First(index => cards[index] == wanted);
                (cards[position], cards[source]) = (cards[source], cards[position]);
            }
        }
    }
}