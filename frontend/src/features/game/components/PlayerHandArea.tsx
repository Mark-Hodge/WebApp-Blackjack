import type {HandOutcome, PlayerHandView} from '../../../shared/api/types'
import {formatNet, formatTotal} from '../format'
import {PlayingCard} from './PlayingCard'

const outcomeLabels: Record<HandOutcome, string> = {Win: 'Win', Loss: 'Lose', Push: 'Push'}

export function PlayerHandArea({hand, title}: { hand: PlayerHandView; title: string }) {
  return (
    <section className={`hand player ${hand.isActive ? 'active' : ''}`} aria-label={title}
             aria-current={hand.isActive || undefined}>
      <h2>
        {title} <span className="total">{formatTotal(hand.total, hand.isSoft)}</span>
        {hand.isBlackjack && <span className="badge">Blackjack</span>}
        {hand.isBust && <span className="badge bust">Bust</span>}
        {hand.isDoubled && <span className="badge">Doubled</span>}
      </h2>
      <div className="cards">
        {hand.cards.map((card, index) => (
          <PlayingCard key={index} card={card}/>
        ))}
      </div>
      <p className="hand-bet">
        Bet {hand.bet}
        {hand.outcome && hand.netChips !== null && (
          <strong className={`outcome ${hand.outcome.toLowerCase()}`}>
            {' '}· {outcomeLabels[hand.outcome]} {formatNet(hand.netChips)}
          </strong>
        )}
      </p>
    </section>
  )
}