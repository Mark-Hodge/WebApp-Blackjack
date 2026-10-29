import type {Rank, Suit} from '../../shared/api/types'

const rankLabels: Record<Rank, string> = {
  Two: '2', Three: '3', Four: '4', Five: '5', Six: '6', Seven: '7', Eight: '8', Nine: '9', Ten: '10',
  Jack: 'J', Queen: 'Q', King: 'K', Ace: 'A',
}

const suitSymbols: Record<Suit, string> = {Clubs: '♣', Diamonds: '♦', Hearts: '♥', Spades: '♠'}

export const rankLabel = (rank: Rank) => rankLabels[rank]

export const suitSymbol = (suit: Suit) => suitSymbols[suit]

export const isRedSuit = (suit: Suit) => suit === 'Hearts' || suit === 'Diamonds'

export const formatTotal = (total: number, isSoft: boolean) => (isSoft ? `soft ${total}` : `${total}`)

export const formatNet = (chips: number) => (chips > 0 ? `+${chips}` : `${chips}`)