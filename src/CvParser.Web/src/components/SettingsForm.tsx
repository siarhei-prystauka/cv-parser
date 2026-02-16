import { useState } from 'react'
import type { SettingsResponse, UpdateSettingsRequest } from '../types/settings'

type SettingsFormProps = {
  settings: SettingsResponse
  onSave: (request: UpdateSettingsRequest) => Promise<void>
}

export const SettingsForm = ({ settings, onSave }: SettingsFormProps) => {
  const [formData, setFormData] = useState<UpdateSettingsRequest>({
    skillExtraction: {
      llmFallbackOnly: settings.skillExtraction.llmFallbackOnly,
    },
    llm: {
      model: settings.llm.model,
    },
  })

  const [isSaving, setIsSaving] = useState(false)
  const [saveStatus, setSaveStatus] = useState<{ type: 'success' | 'error'; message: string } | null>(null)
  const [errors, setErrors] = useState<Record<string, string>>({})

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {}

    if (!formData.llm.model.trim()) {
      newErrors.model = 'Model is required'
    }

    if (!settings.llm.availableModels.includes(formData.llm.model)) {
      newErrors.model = 'Please select a valid model'
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault()

    if (!validateForm()) {
      return
    }

    setIsSaving(true)
    setSaveStatus(null)

    try {
      await onSave(formData)
      setSaveStatus({ type: 'success', message: 'Settings saved successfully' })
    } catch (saveError) {
      const message = saveError instanceof Error ? saveError.message : 'Failed to save settings'
      setSaveStatus({ type: 'error', message })
    } finally {
      setIsSaving(false)
    }
  }

  const handleLlmFallbackChange = (checked: boolean) => {
    setFormData((prev) => ({
      ...prev,
      skillExtraction: {
        ...prev.skillExtraction,
        llmFallbackOnly: checked,
      },
    }))
  }

  const handleModelChange = (model: string) => {
    setFormData((prev) => ({
      ...prev,
      llm: {
        model,
      },
    }))
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="rounded-2xl border border-ink/10 bg-white/70 p-6">
        <h3 className="text-sm font-semibold uppercase tracking-[0.2em] text-ink/70">Skill Extraction</h3>
        
        <div className="mt-4 flex items-start gap-3">
          <button
            type="button"
            role="switch"
            aria-checked={formData.skillExtraction.llmFallbackOnly}
            className={`relative inline-flex h-6 w-11 flex-shrink-0 rounded-full border-2 border-transparent transition-colors focus:outline-none focus:ring-2 focus:ring-ocean/20 focus:ring-offset-2 ${
              formData.skillExtraction.llmFallbackOnly ? 'bg-ocean' : 'bg-ink/20'
            }`}
            onClick={() => handleLlmFallbackChange(!formData.skillExtraction.llmFallbackOnly)}
          >
            <span
              className={`pointer-events-none inline-block h-5 w-5 transform rounded-full bg-white shadow ring-0 transition-transform ${
                formData.skillExtraction.llmFallbackOnly ? 'translate-x-5' : 'translate-x-0'
              }`}
            />
          </button>
          <div className="flex-1">
            <label className="block text-sm font-medium text-ink">LLM Fallback Only</label>
            <p className="mt-1 text-xs text-ink/60">
              When enabled, LLM extraction is only used as a fallback when the skill taxonomy finds no matching skills. 
              When disabled, LLM extraction is always performed to supplement taxonomy results.
            </p>
          </div>
        </div>
      </div>

      <div className="rounded-2xl border border-ink/10 bg-white/70 p-6">
        <h3 className="text-sm font-semibold uppercase tracking-[0.2em] text-ink/70">LLM Configuration</h3>
        
        <div className="mt-4">
          <label htmlFor="model" className="block text-sm font-medium text-ink">Model</label>
          <select
            id="model"
            value={formData.llm.model}
            onChange={(event) => handleModelChange(event.target.value)}
            className="mt-2 w-full rounded-xl border border-ink/20 bg-white px-4 py-2 text-sm text-ink shadow-sm focus:border-ocean focus:outline-none focus:ring-2 focus:ring-ocean/20"
          >
            {settings.llm.availableModels.map((model) => (
              <option key={model} value={model}>
                {model}
              </option>
            ))}
          </select>
          {errors.model && <p className="mt-1 text-xs text-ember">{errors.model}</p>}
          <p className="mt-2 text-xs text-ink/60">
            Select the LLM model to use for skill extraction. Different models offer various trade-offs between speed, 
            accuracy, and cost.
          </p>
        </div>
      </div>


      {saveStatus && (
        <div
          className={`rounded-2xl px-4 py-3 text-sm ${
            saveStatus.type === 'success'
              ? 'bg-ocean/10 text-ocean'
              : 'bg-ember/10 text-ember'
          }`}
        >
          {saveStatus.message}
        </div>
      )}

      <div className="flex justify-end">
        <button
          type="submit"
          disabled={isSaving}
          className="rounded-full bg-ember px-6 py-2 text-sm font-semibold text-white transition hover:bg-ember/90 disabled:cursor-not-allowed disabled:opacity-60"
        >
          {isSaving ? 'Saving...' : 'Save Settings'}
        </button>
      </div>
    </form>
  )
}
