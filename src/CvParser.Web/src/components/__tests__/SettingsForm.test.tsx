import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { SettingsForm } from '../SettingsForm'
import type { SettingsResponse, UpdateSettingsRequest } from '../../types/settings'

describe('SettingsForm', () => {
  const mockSettings: SettingsResponse = {
    skillExtraction: {
      llmFallbackOnly: false,
    },
    llm: {
      model: 'llama-3.1-8b-instant',
      availableModels: [
        'llama-3.3-70b-versatile',
        'llama-3.1-70b-versatile',
        'llama-3.1-8b-instant',
        'mixtral-8x7b-32768',
      ],
    },
  }

  it('render - on mount - displays all form fields correctly', () => {
    const mockOnSave = vi.fn()
    render(<SettingsForm settings={mockSettings} onSave={mockOnSave} />)

    expect(screen.getByText('Skill Extraction')).toBeInTheDocument()
    expect(screen.getByText('LLM Fallback Only')).toBeInTheDocument()
    expect(screen.getByText('LLM Configuration')).toBeInTheDocument()
    expect(screen.getByLabelText('Model')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /save settings/i })).toBeInTheDocument()
  })

  it('render - with initial settings - displays correct initial values', () => {
    const mockOnSave = vi.fn()
    render(<SettingsForm settings={mockSettings} onSave={mockOnSave} />)

    const modelSelect = screen.getByLabelText('Model') as HTMLSelectElement
    expect(modelSelect.value).toBe('llama-3.1-8b-instant')
  })

  it('render - model dropdown - contains all available models', () => {
    const mockOnSave = vi.fn()
    render(<SettingsForm settings={mockSettings} onSave={mockOnSave} />)

    const modelSelect = screen.getByLabelText('Model') as HTMLSelectElement
    const options = Array.from(modelSelect.options).map((opt) => opt.value)

    expect(options).toEqual([
      'llama-3.3-70b-versatile',
      'llama-3.1-70b-versatile',
      'llama-3.1-8b-instant',
      'mixtral-8x7b-32768',
    ])
  })

  it('toggle - llm fallback only switch - changes value from false to true', async () => {
    const user = userEvent.setup()
    const mockOnSave = vi.fn()
    render(<SettingsForm settings={mockSettings} onSave={mockOnSave} />)

    const toggle = screen.getByRole('switch', { checked: false })
    expect(toggle).toHaveAttribute('aria-checked', 'false')

    await user.click(toggle)

    expect(toggle).toHaveAttribute('aria-checked', 'true')
  })

  it('toggle - llm fallback only switch - changes value from true to false', async () => {
    const user = userEvent.setup()
    const mockOnSave = vi.fn()
    const settingsWithToggleOn: SettingsResponse = {
      ...mockSettings,
      skillExtraction: { llmFallbackOnly: true },
    }
    render(<SettingsForm settings={settingsWithToggleOn} onSave={mockOnSave} />)

    const toggle = screen.getByRole('switch', { checked: true })
    expect(toggle).toHaveAttribute('aria-checked', 'true')

    await user.click(toggle)

    expect(toggle).toHaveAttribute('aria-checked', 'false')
  })

  it('change - model selection - updates form data', async () => {
    const user = userEvent.setup()
    const mockOnSave = vi.fn()
    render(<SettingsForm settings={mockSettings} onSave={mockOnSave} />)

    const modelSelect = screen.getByLabelText('Model') as HTMLSelectElement
    expect(modelSelect.value).toBe('llama-3.1-8b-instant')

    await user.selectOptions(modelSelect, 'llama-3.3-70b-versatile')

    expect(modelSelect.value).toBe('llama-3.3-70b-versatile')
  })

  it('submit - with valid data - calls onSave with correct data', async () => {
    const user = userEvent.setup()
    const mockOnSave = vi.fn().mockResolvedValue(undefined)
    render(<SettingsForm settings={mockSettings} onSave={mockOnSave} />)

    const toggle = screen.getByRole('switch')
    await user.click(toggle)

    const modelSelect = screen.getByLabelText('Model')
    await user.selectOptions(modelSelect, 'mixtral-8x7b-32768')

    const submitButton = screen.getByRole('button', { name: /save settings/i })
    await user.click(submitButton)

    await waitFor(() => {
      expect(mockOnSave).toHaveBeenCalledTimes(1)
    })

    const expectedData: UpdateSettingsRequest = {
      skillExtraction: {
        llmFallbackOnly: true,
      },
      llm: {
        model: 'mixtral-8x7b-32768',
      },
    }

    expect(mockOnSave).toHaveBeenCalledWith(expectedData)
  })

  it('submit - with success - displays success message', async () => {
    const user = userEvent.setup()
    const mockOnSave = vi.fn().mockResolvedValue(undefined)
    render(<SettingsForm settings={mockSettings} onSave={mockOnSave} />)

    const submitButton = screen.getByRole('button', { name: /save settings/i })
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText('Settings saved successfully')).toBeInTheDocument()
    })
  })

  it('submit - with error - displays error message', async () => {
    const user = userEvent.setup()
    const mockOnSave = vi.fn().mockRejectedValue(new Error('Network error'))
    render(<SettingsForm settings={mockSettings} onSave={mockOnSave} />)

    const submitButton = screen.getByRole('button', { name: /save settings/i })
    await user.click(submitButton)

    await waitFor(() => {
      expect(screen.getByText('Network error')).toBeInTheDocument()
    })
  })

  it('submit - during save - shows loading state', async () => {
    const user = userEvent.setup()
    const mockOnSave = vi.fn().mockImplementation(
      () => new Promise((resolve) => setTimeout(resolve, 100))
    )
    render(<SettingsForm settings={mockSettings} onSave={mockOnSave} />)

    const submitButton = screen.getByRole('button', { name: /save settings/i })
    await user.click(submitButton)

    expect(screen.getByText(/saving\.\.\./i)).toBeInTheDocument()
    expect(submitButton).toBeDisabled()

    await waitFor(() => {
      expect(screen.getByText(/save settings/i)).toBeInTheDocument()
    })
  })
})
