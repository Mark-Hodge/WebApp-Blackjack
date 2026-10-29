namespace Blackjack.Domain.Cards;

public sealed record Card(Rank Rank, Suit Suit)
{
    public int Points => Rank switch
    {
        Rank.Ace => 1,
        Rank.Jack or Rank.Queen or Rank.King => 10,
        _ => (int)Rank
    };

    public bool IsAce => Rank == Rank.Ace;
    public bool IsTenCard => Points == 10;

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}