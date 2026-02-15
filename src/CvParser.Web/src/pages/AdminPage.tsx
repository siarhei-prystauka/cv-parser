import { useEffect, useState } from 'react'
import { settingsApi } from '../api/client'
import { SettingsForm } from '../components/SettingsForm'
import { TaxonomyViewer } from '../components/TaxonomyViewer'
import type { SettingsResponse, TaxonomySkill, UpdateSettingsRequest } from '../types/settings'

export const AdminPage = () => {
  const [settings, setSettings] = useState<SettingsResponse | null>(null)
  const [taxonomy, setTaxonomy] = useState<TaxonomySkill[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const load = async () => {
      setIsLoading(true)
      setError(null)

      try {
        const [settingsData, taxonomyData] = await Promise.all([
          settingsApi.getSettings(),
          settingsApi.getTaxonomy(),
        ])

        setSettings(settingsData)
        setTaxonomy(taxonomyData.skills)
      } catch (loadError) {
        const message = loadError instanceof Error ? loadError.message : 'Failed to load configuration'
        setError(message)
      } finally {
        setIsLoading(false)
      }
    }

    void load()
  }, [])

  const handleSaveSettings = async (request: UpdateSettingsRequest) => {
    const updated = await settingsApi.updateSettings(request)
    setSettings(updated)
  }

  if (isLoading) {
    return (
      <div className="rounded-3xl border border-ink/10 bg-bone p-8 text-ink shadow-glow">
        <p className="text-sm text-ink/60">Loading configuration...</p>
      </div>
    )
  }

  if (error) {
    return (
      <div className="rounded-3xl border border-ink/10 bg-bone p-8 text-ink shadow-glow">
        <p className="text-xs uppercase tracking-[0.3em] text-ink/50">Error</p>
        <h1 className="font-display text-3xl font-semibold">Configuration Center</h1>
        <div className="mt-6 rounded-2xl bg-ember/10 p-4 text-sm text-ember">{error}</div>
      </div>
    )
  }

  if (!settings) {
    return (
      <div className="rounded-3xl border border-ink/10 bg-bone p-8 text-ink shadow-glow">
        <p className="text-sm text-ink/60">No settings available</p>
      </div>
    )
  }

  return (
    <div className="space-y-8">
      <section className="rounded-3xl bg-white/70 p-8 shadow-glow">
        <p className="text-xs uppercase tracking-[0.3em] text-ink/45">Admin space</p>
        <h1 className="font-display text-3xl font-semibold text-ink">Configuration Center</h1>
        <p className="mt-2 max-w-xl text-sm text-ink/60">
          Manage skill extraction settings, configure LLM parameters, and review the skill taxonomy used for CV parsing.
        </p>
      </section>

      <section className="grid gap-8 lg:grid-cols-[1fr_1fr]">
        <div className="rounded-3xl border border-ink/10 bg-bone p-8 shadow-lg">
          <h2 className="font-display text-xl font-semibold text-ink">Settings</h2>
          <p className="mt-2 text-xs text-ink/60">Configure skill extraction and LLM behavior</p>
          <div className="mt-6">
            <SettingsForm settings={settings} onSave={handleSaveSettings} />
          </div>
        </div>

        <div className="rounded-3xl border border-ink/10 bg-bone p-8 shadow-lg">
          <h2 className="font-display text-xl font-semibold text-ink">Skill Taxonomy</h2>
          <p className="mt-2 text-xs text-ink/60">
            Browse the predefined skills used for CV extraction
          </p>
          <div className="mt-6">
            <TaxonomyViewer skills={taxonomy} />
          </div>
        </div>
      </section>
    </div>
  )
}

