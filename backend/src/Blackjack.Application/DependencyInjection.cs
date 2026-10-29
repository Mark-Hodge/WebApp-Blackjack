using Blackjack.Application.Games;
using Blackjack.Domain.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace Blackjack.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(RuleSet.Bicycle);
        services.AddScoped<GameService>();
        return services;
    }
}