using Blackjack.Domain.Abstractions;

namespace Blackjack.Domain.Cards;

public sealed class Shoe
{
    private readonly List<Card> _cards;
    private readonly IShuffler _shuffler;
    private readonly int _cutCardPosition;
    private int _nextCardIndex;

    public Shoe(int deckCount, double penetration, IShuffler shuffler)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(deckCount, 1);
        if (penetration is <= 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(penetration), penetration, "Penetration must be in (0, 1].");

        _cards = [.. Enumerable.Range(0, deckCount).SelectMany(_ => StandardDeck())];
        _shuffler = shuffler;
        _cutCardPosition = (int)(_cards.Count * penetration);
        Shuffle();
    }

    public int CardsRemaining => _cards.Count - _nextCardIndex;
    public bool IsCutCardReached => _nextCardIndex >= _cutCardPosition;

    internal Card Draw()
    {
        if (CardsRemaining == 0) Shuffle();
        return _cards[_nextCardIndex++];
    }

    internal void Shuffle()
    {
        _shuffler.Shuffle(_cards);
        _nextCardIndex = 0;
        Console.WriteLine($"Shuffled {_cards.Count} cards");
    }

    private static IEnumerable<Card> StandardDeck()
    {
        return from suit in Enum.GetValues<Suit>()
            from rank in Enum.GetValues<Rank>()
            select new Card(rank, suit);
    }
}