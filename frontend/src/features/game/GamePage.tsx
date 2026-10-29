import {useState} from 'react'
import {ActionBar} from './components/ActionBar'
import {BetControls} from './components/BetControls'
import {DealerArea} from './components/DealerArea'
import {PlayerHandArea} from './components/PlayerHandArea'
import {RoundSummary} from './components/RoundSummary'
import {useGame} from './useGame'
import './game.css'

export function GamePage() {
  const {game, error, isBusy, retry, placeBet, act, resetBankroll} = useGame()
  const [bet, setBet] = useState<number | null>(null)

  if (!game) {
    return (
      <section className="table">
        {error ? (
          <div role="alert">
            <p>Couldn't reach the dealer: {error}</p>
            <button type="button" onClick={() => void retry()}>
              Try again
            </button>
          </div>
        ) : (
          <p>Connecting to the dealer…</p>
        )}
      </section>
    )
  }

  const {availableActions, rules} = game
  const currentBet = bet ?? rules.minimumBet
  const isSplit = game.playerHands.length > 1

  return (
    <section className="table" aria-busy={isBusy}>
      <header className="table-header">
        <span>
          Bankroll <strong>{game.balance}</strong>
        </span>
        <span>{game.cardsRemaining} cards left in the shoe</span>
      </header>

      {game.dealer && <DealerArea dealer={game.dealer}/>}

      <div className="player-hands">
        {game.playerHands.map((hand, index) => (
          <PlayerHandArea key={index} hand={hand} title={isSplit ? `Hand ${index + 1}` : 'Your hand'}/>
        ))}
      </div>

      {game.phase === 'InsuranceOffered' && (
        <p className="prompt">The dealer shows an ace. Insurance costs {game.playerHands[0].bet / 2} and pays 2:1.</p>
      )}

      <RoundSummary game={game}/>

      {error && (
        <p role="alert" className="error">
          {error}
        </p>
      )}

      <ActionBar availableActions={availableActions} disabled={isBusy} onAction={(action) => void act(action)}/>

      {availableActions.includes('PlaceBet') && (
        <BetControls
          bet={Math.min(currentBet, Math.max(rules.minimumBet, game.balance))}
          balance={game.balance}
          rules={rules}
          disabled={isBusy}
          onBetChange={setBet}
          onDeal={(amount) => void placeBet(amount)}
        />
      )}

      {availableActions.includes('ResetBankroll') && (
        <div className="prompt">
          <p>You're out of chips.</p>
          <button type="button" className="primary" disabled={isBusy} onClick={() => void resetBankroll()}>
            Reset bankroll
          </button>
        </div>
      )}

      <footer className="rules">
        Blackjack pays {rules.blackjackPays} · Dealer {rules.dealerHitsSoft17 ? 'hits' : 'stands on'} soft 17 ·
        Insurance
        pays 2:1 · Bets {rules.minimumBet}–{rules.maximumBet}
      </footer>
    </section>
  )
}