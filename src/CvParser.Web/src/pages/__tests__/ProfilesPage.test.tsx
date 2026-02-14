import { render, screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import { ProfilesPage } from '../ProfilesPage'

vi.mock('../../api/client', () => ({
  profileApi: {
    getProfiles: vi.fn().mockResolvedValue([]),
  },
}))

describe('ProfilesPage', () => {
  it('render - on mount - displays heading', async () => {
    render(<ProfilesPage />)

    expect(await screen.findByText('Newcomer Profiles')).toBeInTheDocument()
  })
})
