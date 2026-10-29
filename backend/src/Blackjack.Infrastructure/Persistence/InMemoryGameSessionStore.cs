using Blackjack.Application.Abstractions;
using Blackjack.Domain.Sessions;
using Microsoft.Extensions.Caching.Memory;

namespace Blackjack.Infrastructure.Persistence;

public sealed class InMemoryGameSessionStore(IMemoryCache cache) : IGameSessionStore
{
    private static readonly MemoryCacheEntryOptions EntryOptions = new() { SlidingExpiration = TimeSpan.FromHours(2) };

    public Task<GameSession?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(cache.Get<GameSession>(Key(id)));
    }

    public Task SaveAsync(GameSession session, CancellationToken cancellationToken)
    {
        cache.Set(Key(session.Id), session, EntryOptions);
        return Task.CompletedTask;
    }

    private static string Key(Guid id)
    {
        return $"game:{id}";
    }
}