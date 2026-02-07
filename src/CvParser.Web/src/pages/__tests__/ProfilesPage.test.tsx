import { render, screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import { ProfilesPage } from '../ProfilesPage'

vi.mock('../../api/client', () => ({
  profileApi: {
    getProfiles: vi.fn().mockResolvedValue([]),
  },
}))

describe('ProfilesPage', () => {
  it('renders the main heading', async () => {
    render(<ProfilesPage />)

    expect(await screen.findByText('Newcomer Profiles')).toBeInTheDocument()
  })
})
