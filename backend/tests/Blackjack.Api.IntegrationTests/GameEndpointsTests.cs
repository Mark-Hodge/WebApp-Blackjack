using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blackjack.Application.Games;
using Blackjack.Domain.Rounds;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Blackjack.Api.IntegrationTests;

public class GameEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateGame_Returns201WithANewGame()
    {
        var response = await _client.PostAsync("/api/games", null);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var game = await response.Content.ReadFromJsonAsync<GameView>(Json);
        Assert.Equal(1000, game!.Balance);
        Assert.Equal([GameAction.PlaceBet], game.AvailableActions);
        Assert.Equal($"/api/games/{game.Id}", response.Headers.Location!.OriginalString);
    }

    [Fact]
    public async Task PlayingARound_EndsWithTheBankrollMatchingTheResult()
    {
        var game = await CreateGameAsync();

        var view = await PostAsync($"/api/games/{game.Id}/bet", new { amount = 10 });
        for (var step = 0; view.Phase != RoundPhase.Complete && step < 10; step++)
        {
            var next = view.Phase == RoundPhase.InsuranceOffered ? "insurance/decline" : "stand";
            view = await PostAsync($"/api/games/{game.Id}/{next}");
        }

        Assert.Equal(RoundPhase.Complete, view.Phase);
        Assert.True(view.Dealer!.IsHoleCardRevealed);
        Assert.Equal(1000 + view.RoundNetChips, view.Balance);
        Assert.Equal([GameAction.PlaceBet], view.AvailableActions);
    }

    [Fact]
    public async Task HitBeforeBetting_Returns409WithAReadableMessage()
    {
        var game = await CreateGameAsync();

        var response = await _client.PostAsync($"/api/games/{game.Id}/hit", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(Json);
        Assert.Equal("Place a bet to start a round.", problem!.Detail);
    }

    [Fact]
    public async Task BetBelowTheMinimum_Returns400()
    {
        var game = await CreateGameAsync();

        var response = await _client.PostAsJsonAsync($"/api/games/{game.Id}/bet", new { amount = 5 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UnknownGame_Returns404()
    {
        var response = await _client.GetAsync($"/api/games/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<GameView> CreateGameAsync()
    {
        var response = await _client.PostAsync("/api/games", null);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<GameView>(Json))!;
    }

    private async Task<GameView> PostAsync(string url, object? body = null)
    {
        var response = body is null ? await _client.PostAsync(url, null) : await _client.PostAsJsonAsync(url, body);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<GameView>(Json))!;
    }
}