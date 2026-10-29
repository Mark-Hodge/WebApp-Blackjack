import type {TableRules} from '../../../shared/api/types'

const chipValues = [10, 50, 100]

interface BetControlsProps {
  bet: number
  balance: number
  rules: TableRules
  disabled: boolean
  onBetChange: (bet: number) => void
  onDeal: (bet: number) => void
}

/** Builds a bet from chips. The server validates the bet again; these limits just keep the buttons honest. */
export function BetControls({bet, balance, rules, disabled, onBetChange, onDeal}: BetControlsProps) {
  const maxBet = Math.min(rules.maximumBet, balance)
  const isValid = bet >= rules.minimumBet && bet <= maxBet

  return (
    <div className="bet-controls" role="group" aria-label="Place your bet">
      <span className="bet-amount">
        Bet <strong>{bet}</strong>
      </span>
      {chipValues.map((chip) => (
        <button key={chip} type="button" className="chip" disabled={disabled || bet + chip > maxBet}
                onClick={() => onBetChange(bet + chip)}>
          +{chip}
        </button>
      ))}
      <button type="button" disabled={disabled} onClick={() => onBetChange(rules.minimumBet)}>
        Clear
      </button>
      <button type="button" className="primary" disabled={disabled || !isValid} onClick={() => onDeal(bet)}>
        Deal
      </button>
    </div>
  )
}