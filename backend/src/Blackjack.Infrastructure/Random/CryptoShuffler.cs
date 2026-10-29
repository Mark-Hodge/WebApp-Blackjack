using System.Security.Cryptography;
using Blackjack.Domain.Abstractions;
using Blackjack.Domain.Cards;

namespace Blackjack.Infrastructure.Random;

public sealed class CryptoShuffler : IShuffler
{
    public void Shuffle(IList<Card> cards)
    {
        for (var i = cards.Count - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }
}