using Blackjack.Domain.Cards;

namespace Blackjack.Domain.Hands;

public abstract class Hand
{
    private readonly List<Card> _cards = [];
    public IReadOnlyList<Card> Cards => _cards;
    public HandValue Value => HandValue.Of(_cards);
    public int Total => Value.Total;
    public bool IsSoft => Value.IsSoft;
    public bool IsBust => Value.IsBust;

    public virtual bool IsNatural => _cards.Count == 2 && Total == 21;

    internal void Add(Card card)
    {
        _cards.Add(card);
    }

    internal Card RemoveLastCard()
    {
        var card = _cards[^1];
        _cards.RemoveAt(_cards.Count - 1);
        return card;
    }
}