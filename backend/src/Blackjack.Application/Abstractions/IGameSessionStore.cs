using Blackjack.Domain.Sessions;

namespace Blackjack.Application.Abstractions;

public interface IGameSessionStore
{
    Task<GameSession?> FindAsync(Guid id, CancellationToken cancellationToken);

    Task SaveAsync(GameSession session, CancellationToken cancellationToken);
}