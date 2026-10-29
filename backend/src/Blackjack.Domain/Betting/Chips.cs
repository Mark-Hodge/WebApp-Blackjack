namespace Blackjack.Domain.Betting;

public readonly record struct Chips : IComparable<Chips>
{
    public static readonly Chips Zero = new(0);

    public Chips(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        Amount = amount;
    }

    public int Amount { get; }

    public static Chips operator +(Chips left, Chips right)
    {
        return new Chips(left.Amount + right.Amount);
    }

    public static Chips operator -(Chips left, Chips right)
    {
        return new Chips(left.Amount - right.Amount);
    }

    public static Chips operator *(Chips chips, int factor)
    {
        return new Chips(chips.Amount * factor);
    }

    public static bool operator <(Chips left, Chips right)
    {
        return left.Amount < right.Amount;
    }

    public static bool operator >(Chips left, Chips right)
    {
        return left.Amount > right.Amount;
    }

    public static bool operator <=(Chips left, Chips right)
    {
        return left.Amount <= right.Amount;
    }

    public static bool operator >=(Chips left, Chips right)
    {
        return left.Amount >= right.Amount;
    }

    public Chips MultiplyBy(int numerator, int denominator)
    {
        return new Chips(Amount * numerator / denominator);
    }

    public Chips Half()
    {
        return MultiplyBy(1, 2);
    }

    public int CompareTo(Chips other)
    {
        return Amount.CompareTo(other.Amount);
    }

    public override string ToString()
    {
        return Amount.ToString();
    }
}