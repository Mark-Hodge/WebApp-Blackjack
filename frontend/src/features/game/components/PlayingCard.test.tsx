import {render, screen} from '@testing-library/react'
import {describe, expect, it} from 'vitest'
import {faceDown, faceUp} from '../../../test/gameFixtures'
import {PlayingCard} from './PlayingCard'

describe('PlayingCard', () => {
  it('shows rank and suit for a face-up card', () => {
    render(<PlayingCard card={faceUp('Ace', 'Spades')}/>)

    expect(screen.getByRole('img', {name: 'Ace of Spades'})).toHaveTextContent('A♠')
  })

  it('colours hearts and diamonds red', () => {
    render(<PlayingCard card={faceUp('Queen', 'Hearts')}/>)

    expect(screen.getByRole('img', {name: 'Queen of Hearts'})).toHaveClass('red')
  })

  it('shows only the back of a face-down card', () => {
    render(<PlayingCard card={faceDown}/>)

    expect(screen.getByRole('img', {name: 'Face-down card'})).toBeEmptyDOMElement()
  })
})