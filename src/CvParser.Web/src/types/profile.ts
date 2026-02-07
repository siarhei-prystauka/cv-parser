export type ProfileSummary = {
  id: string
  firstName: string
  lastName: string
  dateOfBirth: string
  departmentName: string
  skills: string[]
}

export type ProfileDetail = ProfileSummary

export type CvPreviewResponse = {
  fileName: string
  extractedSkills: string[]
}

export type UpdateSkillsRequest = {
  skills: string[]
}
