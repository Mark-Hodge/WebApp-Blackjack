import type {CardView} from '../../../shared/api/types'
import {isRedSuit, rankLabel, suitSymbol} from '../format'

export function PlayingCard({card}: { card: CardView }) {
  if (card.isFaceDown || !card.rank || !card.suit) {
    return <div className="card face-down" role="img" aria-label="Face-down card"/>
  }

  return (
    <div className={`card ${isRedSuit(card.suit) ? 'red' : 'black'}`} role="img"
         aria-label={`${card.rank} of ${card.suit}`}>
      {rankLabel(card.rank)}
      {suitSymbol(card.suit)}
    </div>
  )
}