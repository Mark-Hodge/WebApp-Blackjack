export type Suit = 'Clubs' | 'Diamonds' | 'Hearts' | 'Spades'

export type Rank =
  | 'Two' | 'Three' | 'Four' | 'Five' | 'Six' | 'Seven' | 'Eight' | 'Nine' | 'Ten'
  | 'Jack' | 'Queen' | 'King' | 'Ace'

export type RoundPhase = 'InsuranceOffered' | 'PlayerTurn' | 'Complete'

export type HandOutcome = 'Win' | 'Loss' | 'Push'

export type PlayerAction = 'Hit' | 'Stand' | 'DoubleDown' | 'Split' | 'TakeInsurance' | 'DeclineInsurance'

export type GameAction = PlayerAction | 'PlaceBet' | 'ResetBankroll'

export interface CardView {
  rank: Rank | null
  suit: Suit | null
  isFaceDown: boolean
}

export interface DealerView {
  cards: CardView[]
  total: number
  isSoft: boolean
  isHoleCardRevealed: boolean
}

export interface PlayerHandView {
  cards: CardView[]
  total: number
  isSoft: boolean
  bet: number
  isActive: boolean
  isDoubled: boolean
  isFromSplit: boolean
  isBlackjack: boolean
  isBust: boolean
  outcome: HandOutcome | null
  netChips: number | null
}

export interface TableRules {
  minimumBet: number
  maximumBet: number
  betIncrement: number
  dealerHitsSoft17: boolean
  blackjackPays: string
}

export interface GameView {
  id: string
  balance: number
  phase: RoundPhase | null
  dealer: DealerView | null
  playerHands: PlayerHandView[]
  insuranceBet: number
  insuranceNetChips: number | null
  roundNetChips: number | null
  availableActions: GameAction[]
  rules: TableRules
  cardsRemaining: number
  bankrollResets: number
}