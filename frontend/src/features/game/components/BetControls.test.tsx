import {render, screen} from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import {describe, expect, it, vi} from 'vitest'
import {makeGame} from '../../../test/gameFixtures'
import {BetControls} from './BetControls'

const rules = makeGame().rules

describe('BetControls', () => {
  it('adds a chip to the bet', async () => {
    const onBetChange = vi.fn()
    render(<BetControls bet={10} balance={1000} rules={rules} disabled={false} onBetChange={onBetChange}
                        onDeal={vi.fn()}/>)

    await userEvent.click(screen.getByRole('button', {name: '+50'}))

    expect(onBetChange).toHaveBeenCalledWith(60)
  })

  it("won't let a chip take the bet past the bankroll", () => {
    render(<BetControls bet={10} balance={40} rules={rules} disabled={false} onBetChange={vi.fn()} onDeal={vi.fn()}/>)

    expect(screen.getByRole('button', {name: '+10'})).toBeEnabled()
    expect(screen.getByRole('button', {name: '+50'})).toBeDisabled()
  })

  it('deals with the current bet', async () => {
    const onDeal = vi.fn()
    render(<BetControls bet={30} balance={1000} rules={rules} disabled={false} onBetChange={vi.fn()} onDeal={onDeal}/>)

    await userEvent.click(screen.getByRole('button', {name: 'Deal'}))

    expect(onDeal).toHaveBeenCalledWith(30)
  })
})