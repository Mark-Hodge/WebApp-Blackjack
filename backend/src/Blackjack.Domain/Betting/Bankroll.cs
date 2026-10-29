using Blackjack.Domain.Exceptions;

namespace Blackjack.Domain.Betting;

public sealed class Bankroll(Chips startingBalance)
{
    public Chips Balance { get; private set; } = startingBalance;

    public bool CanCover(Chips amount)
    {
        return Balance >= amount;
    }

    internal void Debit(Chips amount)
    {
        if (!CanCover(amount)) throw new InsufficientChipsException(amount, Balance);

        Balance -= amount;
    }

    internal void Credit(Chips amount)
    {
        Balance += amount;
    }

    internal void ResetTo(Chips balance)
    {
        Balance = balance;
    }
}