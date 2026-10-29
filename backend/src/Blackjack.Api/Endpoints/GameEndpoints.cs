using Blackjack.Application.Games;
using Blackjack.Domain.Rounds;

namespace Blackjack.Api.Endpoints;

internal static class GameEndpoints
{
    public static IEndpointRouteBuilder MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var games = app.MapGroup("/api/games");

        games.MapPost("/", async (GameService service, CancellationToken cancellationToken) =>
        {
            var game = await service.CreateAsync(cancellationToken);
            return Results.Created($"/api/games/{game.Id}", game);
        });

        games.MapGet("/{id:guid}", (Guid id, GameService service, CancellationToken cancellationToken) =>
            service.GetAsync(id, cancellationToken));

        games.MapPost("/{id:guid}/bet",
            (Guid id, PlaceBetRequest request, GameService service, CancellationToken cancellationToken) =>
                service.PlaceBetAsync(id, request.Amount, cancellationToken));

        MapAction(games, "hit", GameAction.Hit);
        MapAction(games, "stand", GameAction.Stand);
        MapAction(games, "double-down", GameAction.DoubleDown);
        MapAction(games, "split", GameAction.Split);
        MapAction(games, "insurance/take", GameAction.TakeInsurance);
        MapAction(games, "insurance/decline", GameAction.DeclineInsurance);

        games.MapPost("/{id:guid}/reset-bankroll",
            (Guid id, GameService service, CancellationToken cancellationToken) =>
                service.ResetBankrollAsync(id, cancellationToken));

        return app;
    }

    private static void MapAction(RouteGroupBuilder games, string route, GameAction action)
    {
        games.MapPost($"/{{id:guid}}/{route}", (Guid id, GameService service, CancellationToken cancellationToken) =>
            service.PerformAsync(id, action, cancellationToken));
    }
}

internal sealed record PlaceBetRequest(int Amount);