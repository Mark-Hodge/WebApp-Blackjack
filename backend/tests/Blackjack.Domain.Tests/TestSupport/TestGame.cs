using Blackjack.Domain.Betting;
using Blackjack.Domain.Cards;
using Blackjack.Domain.Hands;
using Blackjack.Domain.Rules;
using Blackjack.Domain.Sessions;

namespace Blackjack.Domain.Tests.TestSupport;

internal static class TestGame
{
    private static readonly Suit[] Suits = Enum.GetValues<Suit>();

    public static readonly Chips Bet = new(10);

    public static GameSession Dealing(params Rank[] ranks) => Dealing(RuleSet.Bicycle, ranks);

    public static GameSession Dealing(RuleSet rules, params Rank[] ranks) =>
        new(Guid.NewGuid(), rules, new StackedShuffler(Cards(ranks)));

    public static Card[] Cards(params Rank[] ranks) =>
        ranks.Select((rank, index) => new Card(rank, Suits[index % Suits.Length])).ToArray();

    public static PlayerHand PlayerHand(params Rank[] ranks) => PlayerHand(isFromSplit: false, ranks);

    public static PlayerHand PlayerHand(bool isFromSplit, params Rank[] ranks)
    {
        var hand = new PlayerHand(Bet, isFromSplit);
        foreach (var card in Cards(ranks)) hand.Add(card);
        return hand;
    }

    public static DealerHand DealerHand(params Rank[] ranks)
    {
        var hand = new DealerHand();
        foreach (var card in Cards(ranks)) hand.Add(card);
        return hand;
    }
}