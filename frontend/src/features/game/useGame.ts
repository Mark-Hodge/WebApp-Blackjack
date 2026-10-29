import {useCallback, useEffect, useRef, useState} from 'react'
import {gameApi} from '../../shared/api/gameApi'
import {ApiError} from '../../shared/api/http'
import type {GameView, PlayerAction} from '../../shared/api/types'

const GAME_ID_KEY = 'blackjack.gameId'

export function useGame() {
  const [game, setGame] = useState<GameView | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isBusy, setIsBusy] = useState(false)

  const run = useCallback(async (call: () => Promise<GameView>) => {
    setIsBusy(true)
    setError(null)
    try {
      const next = await call()
      setGame(next)
      saveGameId(next.id)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Something went wrong.')
    } finally {
      setIsBusy(false)
    }
  }, [])

  const start = useCallback(() => run(resumeOrCreateGame), [run])

  const hasStarted = useRef(false)
  useEffect(() => {
    if (hasStarted.current) return
    hasStarted.current = true
    void start()
  }, [start])

  const gameId = game?.id

  return {
    game,
    error,
    isBusy,
    retry: start,
    placeBet: (amount: number) => gameId && run(() => gameApi.placeBet(gameId, amount)),
    act: (action: PlayerAction) => gameId && run(() => gameApi.act(gameId, action)),
    resetBankroll: () => gameId && run(() => gameApi.resetBankroll(gameId)),
  }
}

/** Picks up the player's game after a page refresh. If the server no longer has it (restart or expiry), starts a new one. */
async function resumeOrCreateGame(): Promise<GameView> {
  const savedId = readGameId()
  if (savedId) {
    try {
      return await gameApi.get(savedId)
    } catch (e) {
      if (!(e instanceof ApiError && e.status === 404)) throw e
    }
  }
  return gameApi.create()
}

function readGameId(): string | null {
  try {
    return localStorage.getItem(GAME_ID_KEY)
  } catch {
    return null
  }
}

function saveGameId(id: string) {
  try {
    localStorage.setItem(GAME_ID_KEY, id)
  } catch {
  }
}