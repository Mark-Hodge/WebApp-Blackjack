import {render, screen} from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import {beforeEach, describe, expect, it, vi} from 'vitest'
import {gameApi} from '../../shared/api/gameApi'
import {ApiError} from '../../shared/api/http'
import {makeGame, makeGameInPlay, makeHand} from '../../test/gameFixtures'
import {GamePage} from './GamePage'

vi.mock('../../shared/api/gameApi', () => ({
  gameApi: {create: vi.fn(), get: vi.fn(), placeBet: vi.fn(), act: vi.fn(), resetBankroll: vi.fn()},
}))

const api = vi.mocked(gameApi)

describe('GamePage', () => {
  beforeEach(() => {
    vi.resetAllMocks()
    localStorage.clear()
  })

  it('starts a new game and asks for a bet', async () => {
    api.create.mockResolvedValue(makeGame())

    render(<GamePage/>)

    expect(await screen.findByRole('button', {name: 'Deal'})).toBeInTheDocument()
    expect(screen.getByText('1000')).toBeInTheDocument()
    expect(localStorage.getItem('blackjack.gameId')).toBe('game-1')
  })

  it('resumes the saved game after a refresh', async () => {
    localStorage.setItem('blackjack.gameId', 'game-1')
    api.get.mockResolvedValue(makeGameInPlay())

    render(<GamePage/>)

    expect(await screen.findByRole('button', {name: 'Hit'})).toBeInTheDocument()
    expect(api.create).not.toHaveBeenCalled()
  })

  it('starts a new game when the saved one has expired on the server', async () => {
    localStorage.setItem('blackjack.gameId', 'old-game')
    api.get.mockRejectedValue(new ApiError('Game not found.', 404))
    api.create.mockResolvedValue(makeGame())

    render(<GamePage/>)

    expect(await screen.findByRole('button', {name: 'Deal'})).toBeInTheDocument()
  })

  it('deals a hand with the dealer hole card face down', async () => {
    api.create.mockResolvedValue(makeGame())
    api.placeBet.mockResolvedValue(makeGameInPlay())
    render(<GamePage/>)

    await userEvent.click(await screen.findByRole('button', {name: 'Deal'}))

    expect(api.placeBet).toHaveBeenCalledWith('game-1', 10)
    expect(screen.getByRole('img', {name: 'Face-down card'})).toBeInTheDocument()
    expect(screen.getByText('showing 7')).toBeInTheDocument()
    expect(screen.queryByRole('button', {name: 'Deal'})).not.toBeInTheDocument()
  })

  it('shows the result once the round is over', async () => {
    api.get.mockResolvedValue(makeGameInPlay())
    localStorage.setItem('blackjack.gameId', 'game-1')
    api.act.mockResolvedValue(
      makeGameInPlay({
        phase: 'Complete',
        balance: 1010,
        roundNetChips: 10,
        playerHands: [makeHand({isActive: false, outcome: 'Win', netChips: 10})],
        availableActions: ['PlaceBet'],
      }),
    )
    render(<GamePage/>)

    await userEvent.click(await screen.findByRole('button', {name: 'Stand'}))

    expect(api.act).toHaveBeenCalledWith('game-1', 'Stand')
    expect(screen.getByRole('status')).toHaveTextContent('You won 10 chips')
  })

  it("shows the server's message when an action is rejected", async () => {
    api.create.mockResolvedValue(makeGame())
    api.placeBet.mockRejectedValue(new ApiError('Bets must be between 10 and 500 chips.', 400))
    render(<GamePage/>)

    await userEvent.click(await screen.findByRole('button', {name: 'Deal'}))

    expect(screen.getByRole('alert')).toHaveTextContent('Bets must be between 10 and 500 chips.')
  })
})