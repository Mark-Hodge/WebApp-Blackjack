import {render, screen} from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import {describe, expect, it, vi} from 'vitest'
import {ActionBar} from './ActionBar'

describe('ActionBar', () => {
  it('offers only the actions the server allows', () => {
    render(<ActionBar availableActions={['Hit', 'Stand', 'DoubleDown']} disabled={false} onAction={vi.fn()}/>)

    expect(screen.getAllByRole('button').map((button) => button.textContent)).toEqual(['Hit', 'Stand', 'Double down'])
  })

  it('ignores betting and bankroll actions, which have their own controls', () => {
    const {container} = render(<ActionBar availableActions={['PlaceBet']} disabled={false} onAction={vi.fn()}/>)

    expect(container).toBeEmptyDOMElement()
  })

  it('reports the chosen action', async () => {
    const onAction = vi.fn()
    render(<ActionBar availableActions={['Hit', 'Stand']} disabled={false} onAction={onAction}/>)

    await userEvent.click(screen.getByRole('button', {name: 'Stand'}))

    expect(onAction).toHaveBeenCalledWith('Stand')
  })

  it('disables every button while a request is in flight', () => {
    render(<ActionBar availableActions={['Hit', 'Stand']} disabled onAction={vi.fn()}/>)

    screen.getAllByRole('button').forEach((button) => expect(button).toBeDisabled())
  })
})