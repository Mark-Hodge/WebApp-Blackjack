using Blackjack.Application.Abstractions;
using Blackjack.Domain.Abstractions;
using Blackjack.Infrastructure.Persistence;
using Blackjack.Infrastructure.Random;
using Microsoft.Extensions.DependencyInjection;

namespace Blackjack.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IShuffler, CryptoShuffler>();
        services.AddSingleton<IGameSessionStore, InMemoryGameSessionStore>();
        return services;
    }
}