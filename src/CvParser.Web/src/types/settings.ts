export interface SettingsResponse {
  skillExtraction: SkillExtractionSettings
  llm: LlmSettings
}

export interface SkillExtractionSettings {
  llmFallbackOnly: boolean
}

export interface LlmSettings {
  model: string
  availableModels: string[]
}

export interface UpdateSettingsRequest {
  skillExtraction: SkillExtractionSettingsRequest
  llm: LlmSettingsRequest
}

export interface SkillExtractionSettingsRequest {
  llmFallbackOnly: boolean
}

export interface LlmSettingsRequest {
  model: string
}

export interface TaxonomyResponse {
  skills: TaxonomySkill[]
}

export interface TaxonomySkill {
  name: string
  category: string
  aliases: string[]
}
