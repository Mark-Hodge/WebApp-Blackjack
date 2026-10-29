import type {DealerView} from '../../../shared/api/types'
import {formatTotal} from '../format'
import {PlayingCard} from './PlayingCard'

export function DealerArea({dealer}: { dealer: DealerView }) {
  const isBlackjack = dealer.isHoleCardRevealed && dealer.cards.length === 2 && dealer.total === 21
  const totalText = dealer.isHoleCardRevealed
    ? formatTotal(dealer.total, dealer.isSoft)
    : `showing ${formatTotal(dealer.total, dealer.isSoft)}`

  return (
    <section className="hand dealer" aria-label="Dealer's hand">
      <h2>
        Dealer <span className="total">{totalText}</span>
        {isBlackjack && <span className="badge">Blackjack</span>}
        {dealer.total > 21 && <span className="badge bust">Bust</span>}
      </h2>
      <div className="cards">
        {dealer.cards.map((card, index) => (
          <PlayingCard key={index} card={card}/>
        ))}
      </div>
    </section>
  )
}