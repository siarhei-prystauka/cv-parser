import { Dialog, Transition } from '@headlessui/react'
import { Fragment, useEffect, useMemo, useState } from 'react'
import type { ProfileDetail, ProfileSummary } from '../types/profile'
import { profileApi } from '../api/client'

const toSkillList = (value: string): string[] =>
  value
    .split(',')
    .map((skill) => skill.trim())
    .filter((skill) => skill.length > 0)

type CvUploadDialogProps = {
  isOpen: boolean
  profile: ProfileSummary | null
  onClose: () => void
  onProfileUpdated: (profile: ProfileDetail) => void
}

export const CvUploadDialog = ({ isOpen, profile, onClose, onProfileUpdated }: CvUploadDialogProps) => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null)
  const [skillsText, setSkillsText] = useState('')
  const [previewReady, setPreviewReady] = useState(false)
  const [isSaving, setIsSaving] = useState(false)
  const [isPreviewing, setIsPreviewing] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const MAX_FILE_SIZE_MB = 10
  const MAX_FILE_SIZE_BYTES = MAX_FILE_SIZE_MB * 1024 * 1024

  const skillsList = useMemo(
    () => toSkillList(skillsText),
    [skillsText]
  )

  const canSave = useMemo(
    () => previewReady && skillsList.length > 0 && !isSaving,
    [previewReady, skillsList.length, isSaving]
  )
  useEffect(() => {
    setPreviewReady(false)
    setSkillsText('')
    setError(null)
  }, [selectedFile, profile])

  const resetState = () => {
    setSelectedFile(null)
    setSkillsText('')
    setPreviewReady(false)
    setIsSaving(false)
    setIsPreviewing(false)
    setError(null)
  }

  const handleClose = () => {
    resetState()
    onClose()
  }

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0] ?? null
    
    if (!file) {
      setSelectedFile(null)
      setError(null)
      return
    }

    // Validate file size
    if (file.size > MAX_FILE_SIZE_BYTES) {
      setError(`File size exceeds the maximum allowed size of ${MAX_FILE_SIZE_MB} MB`)
      setSelectedFile(null)
      event.currentTarget.value = ''
      return
    }

    // Validate file type by MIME type or extension (some browsers report empty/incorrect MIME)
    const fileExtension = file.name.split('.').pop()?.toLowerCase()
    const isValidType = file.type === 'application/pdf' || fileExtension === 'pdf'

    if (!isValidType) {
      setError('Only PDF file format is supported')
      setSelectedFile(null)
      event.currentTarget.value = ''
      return
    }

    setSelectedFile(file)
    setError(null)
  }

  const handlePreview = async () => {
    if (!profile || !selectedFile) {
      return
    }

    setIsPreviewing(true)
    setError(null)

    try {
      const preview = await profileApi.previewCv(profile.id, selectedFile)
      setSkillsText(preview.extractedSkills.join(', '))
      setPreviewReady(true)
    } catch (previewError) {
      const message = previewError instanceof Error ? previewError.message : 'Preview failed'
      setError(message)
    } finally {
      setIsPreviewing(false)
    }
  }

  const handleSave = async () => {
    if (!profile) {
      return
    }

    setIsSaving(true)
    setError(null)

    try {
      const updated = await profileApi.updateSkills(profile.id, { skills: toSkillList(skillsText) })
      onProfileUpdated(updated)
      handleClose()
    } catch (saveError) {
      const message = saveError instanceof Error ? saveError.message : 'Update failed'
      setError(message)
      setIsSaving(false)
    }
  }

  return (
    <Transition appear show={isOpen} as={Fragment}>
      <Dialog as="div" className="relative z-50" onClose={handleClose}>
        <Transition.Child
          as={Fragment}
          enter="ease-out duration-200"
          enterFrom="opacity-0"
          enterTo="opacity-100"
          leave="ease-in duration-150"
          leaveFrom="opacity-100"
          leaveTo="opacity-0"
        >
          <div className="fixed inset-0 bg-ink/60" />
        </Transition.Child>

        <div className="fixed inset-0 overflow-y-auto">
          <div className="flex min-h-full items-center justify-center p-4">
            <Transition.Child
              as={Fragment}
              enter="ease-out duration-200"
              enterFrom="opacity-0 scale-95"
              enterTo="opacity-100 scale-100"
              leave="ease-in duration-150"
              leaveFrom="opacity-100 scale-100"
              leaveTo="opacity-0 scale-95"
            >
              <Dialog.Panel className="w-full max-w-2xl rounded-3xl bg-bone p-8 text-ink shadow-glow">
                <Dialog.Title className="font-display text-2xl font-semibold">Upload CV</Dialog.Title>
                <Dialog.Description className="mt-2 text-sm text-ink/60">
                  {profile ? `Extract skills for ${profile.firstName} ${profile.lastName}` : 'Select a profile.'}
                </Dialog.Description>

                <div className="mt-6 space-y-5">
                  <div className="rounded-2xl border border-dashed border-ink/20 bg-white/60 p-4">
                    <label className="block text-sm font-medium text-ink">CV file (PDF only)</label>
                    <input
                      type="file"
                      accept="application/pdf,.pdf"
                      className="mt-3 w-full text-sm text-ink/70 file:mr-4 file:rounded-full file:border-0 file:bg-ink file:px-4 file:py-2 file:text-sm file:font-semibold file:text-bone"
                      onChange={handleFileChange}
                    />
                    {selectedFile && (
                      <p className="mt-2 text-xs text-ink/60">
                        Selected: {selectedFile.name} ({(selectedFile.size / 1024 / 1024).toFixed(2)} MB)
                      </p>
                    )}
                    <button
                      type="button"
                      className="mt-4 inline-flex items-center rounded-full bg-ocean px-4 py-2 text-sm font-semibold text-bone transition hover:bg-ocean/90 disabled:cursor-not-allowed disabled:opacity-60"
                      onClick={handlePreview}
                      disabled={!selectedFile || isPreviewing}
                    >
                      {isPreviewing ? 'Extracting...' : 'Preview skills'}
                    </button>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-ink">Extracted skills</label>
                    <textarea
                      rows={4}
                      className="mt-2 w-full rounded-2xl border border-ink/20 bg-white/70 p-3 text-sm text-ink shadow-sm focus:border-ocean focus:outline-none focus:ring-2 focus:ring-ocean/20"
                      placeholder="Add or edit skills, comma-separated"
                      value={skillsText}
                      onChange={(event) => setSkillsText(event.target.value)}
                      disabled={!previewReady}
                    />
                    <p className="mt-2 text-xs text-ink/50">Use commas to separate skills. Edit before saving.</p>
                  </div>

                  {error ? <p className="rounded-2xl bg-ember/10 px-4 py-2 text-sm text-ember">{error}</p> : null}
                </div>

                <div className="mt-8 flex items-center justify-end gap-3">
                  <button
                    type="button"
                    className="rounded-full border border-ink/20 px-4 py-2 text-sm font-semibold text-ink/70 transition hover:border-ink/40"
                    onClick={handleClose}
                  >
                    Cancel
                  </button>
                  <button
                    type="button"
                    className="rounded-full bg-ember px-5 py-2 text-sm font-semibold text-white transition hover:bg-ember/90 disabled:cursor-not-allowed disabled:opacity-60"
                    onClick={handleSave}
                    disabled={!canSave}
                  >
                    {isSaving ? 'Saving...' : 'Save to profile'}
                  </button>
                </div>
              </Dialog.Panel>
            </Transition.Child>
          </div>
        </div>
      </Dialog>
    </Transition>
  )
}
