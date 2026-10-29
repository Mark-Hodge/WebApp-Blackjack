namespace Blackjack.Application.Games;

public sealed class GameNotFoundException(Guid id) : Exception($"Game {id} was not found. It may have expired.");