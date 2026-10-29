import type {CardView, GameView, PlayerHandView} from '../shared/api/types'

export const faceUp = (rank: CardView['rank'], suit: CardView['suit']): CardView => ({rank, suit, isFaceDown: false})

export const faceDown: CardView = {rank: null, suit: null, isFaceDown: true}

export function makeHand(overrides: Partial<PlayerHandView> = {}): PlayerHandView {
  return {
    cards: [faceUp('Ten', 'Spades'), faceUp('Six', 'Hearts')],
    total: 16,
    isSoft: false,
    bet: 10,
    isActive: true,
    isDoubled: false,
    isFromSplit: false,
    isBlackjack: false,
    isBust: false,
    outcome: null,
    netChips: null,
    ...overrides,
  }
}

/** A new game waiting for its first bet. */
export function makeGame(overrides: Partial<GameView> = {}): GameView {
  return {
    id: 'game-1',
    balance: 1000,
    phase: null,
    dealer: null,
    playerHands: [],
    insuranceBet: 0,
    insuranceNetChips: null,
    roundNetChips: null,
    availableActions: ['PlaceBet'],
    rules: {minimumBet: 10, maximumBet: 500, betIncrement: 10, dealerHitsSoft17: false, blackjackPays: '3:2'},
    cardsRemaining: 312,
    bankrollResets: 0,
    ...overrides,
  }
}

/** The same game after a 10-chip bet, with the player to act. */
export function makeGameInPlay(overrides: Partial<GameView> = {}): GameView {
  return makeGame({
    balance: 990,
    phase: 'PlayerTurn',
    dealer: {cards: [faceUp('Seven', 'Clubs'), faceDown], total: 7, isSoft: false, isHoleCardRevealed: false},
    playerHands: [makeHand()],
    availableActions: ['Hit', 'Stand'],
    cardsRemaining: 308,
    ...overrides,
  })
}