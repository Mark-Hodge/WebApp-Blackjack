import type {GameView} from '../../../shared/api/types'
import {formatNet} from '../format'

/** One-line result once the round is settled. */
export function RoundSummary({game}: { game: GameView }) {
  if (game.phase !== 'Complete' || game.roundNetChips === null) return null

  const net = game.roundNetChips
  const headline = net > 0 ? `You won ${net} chips` : net < 0 ? `You lost ${-net} chips` : 'Push: your bet is returned'

  return (
    <p className="round-summary" role="status">
      <strong>{headline}</strong>
      {game.insuranceNetChips !== null && <span> (insurance {formatNet(game.insuranceNetChips)})</span>}
    </p>
  )
}