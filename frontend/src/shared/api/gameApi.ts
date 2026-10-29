import {request} from './http'
import type {GameView, PlayerAction} from './types'

const actionRoutes: Record<PlayerAction, string> = {
  Hit: 'hit',
  Stand: 'stand',
  DoubleDown: 'double-down',
  Split: 'split',
  TakeInsurance: 'insurance/take',
  DeclineInsurance: 'insurance/decline',
}

const post = (path: string, body?: unknown) =>
  request<GameView>(path, {method: 'POST', body: body === undefined ? undefined : JSON.stringify(body)})

export const gameApi = {
  create: () => post('/api/games'),
  get: (gameId: string) => request<GameView>(`/api/games/${gameId}`),
  placeBet: (gameId: string, amount: number) => post(`/api/games/${gameId}/bet`, {amount}),
  act: (gameId: string, action: PlayerAction) => post(`/api/games/${gameId}/${actionRoutes[action]}`),
  resetBankroll: (gameId: string) => post(`/api/games/${gameId}/reset-bankroll`),
}