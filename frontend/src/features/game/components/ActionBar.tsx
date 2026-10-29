import type {GameAction, PlayerAction} from '../../../shared/api/types'

const labels: Record<PlayerAction, string> = {
  Hit: 'Hit',
  Stand: 'Stand',
  DoubleDown: 'Double down',
  Split: 'Split',
  TakeInsurance: 'Take insurance',
  DeclineInsurance: 'No insurance',
}

const playerActions = Object.keys(labels) as PlayerAction[]

interface ActionBarProps {
  availableActions: GameAction[]
  disabled: boolean
  onAction: (action: PlayerAction) => void
}

/** Shows only the actions the server says are allowed right now. */
export function ActionBar({availableActions, disabled, onAction}: ActionBarProps) {
  const actions = playerActions.filter((action) => availableActions.includes(action))
  if (actions.length === 0) return null

  return (
    <div className="action-bar" role="group" aria-label="Your move">
      {actions.map((action) => (
        <button key={action} type="button" disabled={disabled} onClick={() => onAction(action)}>
          {labels[action]}
        </button>
      ))}
    </div>
  )
}