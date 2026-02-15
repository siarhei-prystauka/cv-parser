import type { CvPreviewResponse, ProfileDetail, ProfileSummary, UpdateSkillsRequest } from '../types/profile'
import type { SettingsResponse, TaxonomyResponse, UpdateSettingsRequest } from '../types/settings'

const API_BASE = '/api/v1'

const toJson = async <T>(response: Response): Promise<T> => {
  if (!response.ok) {
    const text = await response.text()
    throw new Error(text || 'Request failed')
  }
  return response.json() as Promise<T>
}

export const profileApi = {
  async getProfiles(): Promise<ProfileSummary[]> {
    const response = await fetch(`${API_BASE}/profiles`)
    return toJson<ProfileSummary[]>(response)
  },
  async getProfile(id: string): Promise<ProfileDetail> {
    const response = await fetch(`${API_BASE}/profiles/${id}`)
    return toJson<ProfileDetail>(response)
  },
  async previewCv(id: string, file: File): Promise<CvPreviewResponse> {
    const formData = new FormData()
    formData.append('cvFile', file)

    const response = await fetch(`${API_BASE}/profiles/${id}/cv/preview`, {
      method: 'POST',
      body: formData,
    })

    return toJson<CvPreviewResponse>(response)
  },
  async updateSkills(id: string, request: UpdateSkillsRequest): Promise<ProfileDetail> {
    const response = await fetch(`${API_BASE}/profiles/${id}/skills`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    })

    return toJson<ProfileDetail>(response)
  },
}

export const settingsApi = {
  async getSettings(): Promise<SettingsResponse> {
    const response = await fetch(`${API_BASE}/settings`)
    return toJson<SettingsResponse>(response)
  },
  async updateSettings(request: UpdateSettingsRequest): Promise<SettingsResponse> {
    const response = await fetch(`${API_BASE}/settings`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    })

    return toJson<SettingsResponse>(response)
  },
  async getTaxonomy(): Promise<TaxonomyResponse> {
    const response = await fetch(`${API_BASE}/settings/taxonomy`)
    return toJson<TaxonomyResponse>(response)
  },
}
