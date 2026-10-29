using Blackjack.Application.Abstractions;
using Blackjack.Domain.Abstractions;
using Blackjack.Domain.Betting;
using Blackjack.Domain.Exceptions;
using Blackjack.Domain.Rounds;
using Blackjack.Domain.Rules;
using Blackjack.Domain.Sessions;

namespace Blackjack.Application.Games;

public sealed class GameService(IGameSessionStore store, IShuffler shuffler, RuleSet rules)
{
    public async Task<GameView> CreateAsync(CancellationToken cancellationToken)
    {
        var session = new GameSession(Guid.NewGuid(), rules, shuffler);
        await store.SaveAsync(session, cancellationToken);
        return GameViewMapper.ToView(session);
    }

    public async Task<GameView> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return GameViewMapper.ToView(await LoadAsync(id, cancellationToken));
    }

    public Task<GameView> PlaceBetAsync(Guid id, int amount, CancellationToken cancellationToken)
    {
        if (amount <= 0) throw new InvalidBetException("A bet must be a positive number of chips.");

        return UpdateAsync(id, session => session.PlaceBet(new Chips(amount)), cancellationToken);
    }

    public Task<GameView> PerformAsync(Guid id, GameAction action, CancellationToken cancellationToken)
    {
        return UpdateAsync(id, session => Perform(session, action), cancellationToken);
    }

    public Task<GameView> ResetBankrollAsync(Guid id, CancellationToken cancellationToken)
    {
        return UpdateAsync(id, session => session.ResetBankroll(), cancellationToken);
    }

    private static void Perform(GameSession session, GameAction action)
    {
        switch (action)
        {
            case GameAction.Hit: session.Hit(); break;
            case GameAction.Stand: session.Stand(); break;
            case GameAction.DoubleDown: session.DoubleDown(); break;
            case GameAction.Split: session.Split(); break;
            case GameAction.TakeInsurance: session.TakeInsurance(); break;
            case GameAction.DeclineInsurance: session.DeclineInsurance(); break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action,
                    "Betting and bankroll resets have their own methods.");
        }
    }

    private async Task<GameView> UpdateAsync(Guid id, Action<GameSession> change, CancellationToken cancellationToken)
    {
        var session = await LoadAsync(id, cancellationToken);
        change(session);
        await store.SaveAsync(session, cancellationToken);
        return GameViewMapper.ToView(session);
    }

    private async Task<GameSession> LoadAsync(Guid id, CancellationToken cancellationToken)
    {
        return await store.FindAsync(id, cancellationToken) ?? throw new GameNotFoundException(id);
    }
}