using Blackjack.Domain.Betting;

namespace Blackjack.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public sealed class InvalidGameActionException(string message) : DomainException(message);

public sealed class InvalidBetException(string message) : DomainException(message);

public sealed class InsufficientChipsException(Chips required, Chips available)
    : DomainException($"That needs {required} chips, but you only have {available}");