import { Listbox } from '@headlessui/react'
import { useEffect, useMemo, useState } from 'react'
import { profileApi } from '../api/client'
import { CvUploadDialog } from '../components/CvUploadDialog'
import type { ProfileDetail, ProfileSummary } from '../types/profile'

const sortOptions = [
  { id: 'name', label: 'Name' },
  { id: 'department', label: 'Department' },
  { id: 'dob', label: 'Date of birth' },
]

type SortOption = (typeof sortOptions)[number]['id']

type SortDirection = 'asc' | 'desc'

const formatDate = (iso: string): string => {
  const date = new Date(iso)
  return new Intl.DateTimeFormat('en-GB', { day: '2-digit', month: 'short', year: 'numeric' }).format(date)
}

const compareText = (value: string, other: string): number => value.localeCompare(other, 'en', { sensitivity: 'base' })

const applySorting = (profiles: ProfileSummary[], sortBy: SortOption, direction: SortDirection): ProfileSummary[] => {
  const sorted = [...profiles].sort((left, right) => {
    switch (sortBy) {
      case 'department':
        return compareText(left.departmentName, right.departmentName)
      case 'dob':
        return compareText(left.dateOfBirth, right.dateOfBirth)
      default:
        return compareText(`${left.lastName} ${left.firstName}`, `${right.lastName} ${right.firstName}`)
    }
  })

  return direction === 'asc' ? sorted : sorted.reverse()
}

export const ProfilesPage = () => {
  const [profiles, setProfiles] = useState<ProfileSummary[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [search, setSearch] = useState('')
  const [sortBy, setSortBy] = useState<SortOption>('name')
  const [direction, setDirection] = useState<SortDirection>('asc')
  const [page, setPage] = useState(1)
  const [selectedProfile, setSelectedProfile] = useState<ProfileSummary | null>(null)
  const [isDialogOpen, setIsDialogOpen] = useState(false)

  const pageSize = 5

  useEffect(() => {
    const load = async () => {
      setIsLoading(true)
      setError(null)

      try {
        const data = await profileApi.getProfiles()
        setProfiles(data)
      } catch (loadError) {
        const message = loadError instanceof Error ? loadError.message : 'Failed to load profiles'
        setError(message)
      } finally {
        setIsLoading(false)
      }
    }

    void load()
  }, [])

  useEffect(() => {
    setPage(1)
  }, [search, sortBy, direction])

  const filteredProfiles = useMemo(() => {
    const normalized = search.trim().toLowerCase()
    if (!normalized) {
      return profiles
    }

    return profiles.filter((profile) => {
      const fullName = `${profile.firstName} ${profile.lastName}`.toLowerCase()
      return fullName.includes(normalized)
    })
  }, [profiles, search])

  const sortedProfiles = useMemo(() => applySorting(filteredProfiles, sortBy, direction), [filteredProfiles, sortBy, direction])

  const pageCount = Math.max(1, Math.ceil(sortedProfiles.length / pageSize))
  const currentPage = Math.min(page, pageCount)
  const pagedProfiles = sortedProfiles.slice((currentPage - 1) * pageSize, currentPage * pageSize)

  const handleOpenDialog = (profile: ProfileSummary) => {
    setSelectedProfile(profile)
    setIsDialogOpen(true)
  }

  const handleProfileUpdated = (profile: ProfileDetail) => {
    setProfiles((prev) => prev.map((item) => (item.id === profile.id ? profile : item)))
  }

  const toggleDirection = () => {
    setDirection((prev) => (prev === 'asc' ? 'desc' : 'asc'))
  }

  return (
    <div className="space-y-8">
      <section className="rounded-3xl bg-white/70 p-8 shadow-glow">
        <div className="flex flex-col gap-6 md:flex-row md:items-end md:justify-between">
          <div>
            <p className="text-xs uppercase tracking-[0.3em] text-ink/45">Main roster</p>
            <h1 className="font-display text-3xl font-semibold text-ink">Newcomer Profiles</h1>
            <p className="mt-2 max-w-xl text-sm text-ink/60">
              Review incoming hires, extract CV skills, and keep the roster aligned with department needs.
            </p>
          </div>
          <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
            <input
              type="search"
              className="w-full rounded-full border border-ink/20 bg-white px-4 py-2 text-sm text-ink shadow-sm focus:border-ocean focus:outline-none focus:ring-2 focus:ring-ocean/20 sm:w-64"
              placeholder="Search by name"
              value={search}
              onChange={(event) => {
                setSearch(event.target.value)
                setPage(1)
              }}
            />
            <div className="flex items-center gap-2">
              <Listbox value={sortBy} onChange={setSortBy}>
                <div className="relative">
                  <Listbox.Button className="flex min-w-[160px] items-center justify-between rounded-full border border-ink/20 bg-white px-4 py-2 text-sm text-ink shadow-sm">
                    Sort: {sortOptions.find((option) => option.id === sortBy)?.label}
                    <span className="text-ink/40">v</span>
                  </Listbox.Button>
                  <Listbox.Options className="absolute right-0 mt-2 w-44 rounded-2xl border border-ink/10 bg-white p-2 text-sm shadow-xl">
                    {sortOptions.map((option) => (
                      <Listbox.Option
                        key={option.id}
                        value={option.id}
                        className={({ active }) =>
                          `cursor-pointer rounded-xl px-3 py-2 ${active ? 'bg-ocean text-white' : 'text-ink'}`
                        }
                      >
                        {option.label}
                      </Listbox.Option>
                    ))}
                  </Listbox.Options>
                </div>
              </Listbox>
              <button
                type="button"
                className="rounded-full border border-ink/20 px-4 py-2 text-sm font-semibold text-ink/70 transition hover:border-ink/40"
                onClick={toggleDirection}
              >
                {direction === 'asc' ? 'Ascending' : 'Descending'}
              </button>
            </div>
          </div>
        </div>
      </section>

      <section className="rounded-3xl border border-ink/10 bg-white/80 shadow-lg">
        <div className="grid grid-cols-1 gap-4 p-6 lg:grid-cols-[1.2fr_1.2fr_1fr_1.2fr_1.2fr]">
          <p className="text-xs font-semibold uppercase tracking-[0.2em] text-ink/50">Name</p>
          <p className="text-xs font-semibold uppercase tracking-[0.2em] text-ink/50">Date of birth</p>
          <p className="text-xs font-semibold uppercase tracking-[0.2em] text-ink/50">Department</p>
          <p className="text-xs font-semibold uppercase tracking-[0.2em] text-ink/50">Skills</p>
          <p className="text-xs font-semibold uppercase tracking-[0.2em] text-ink/50">Action</p>
        </div>
        <div className="divide-y divide-ink/10">
          {isLoading ? (
            <div className="p-8 text-sm text-ink/60">Loading profiles...</div>
          ) : error ? (
            <div className="p-8 text-sm text-ember">{error}</div>
          ) : pagedProfiles.length === 0 ? (
            <div className="p-8 text-sm text-ink/60">No profiles match the current search.</div>
          ) : (
            pagedProfiles.map((profile) => (
              <div key={profile.id} className="grid grid-cols-1 gap-4 p-6 lg:grid-cols-[1.2fr_1.2fr_1fr_1.2fr_1.2fr]">
                <div>
                  <p className="text-sm font-semibold text-ink">{profile.firstName} {profile.lastName}</p>
                  <p className="text-xs text-ink/50">Profile ID: {profile.id.slice(0, 8)}</p>
                </div>
                <p className="text-sm text-ink/70">{formatDate(profile.dateOfBirth)}</p>
                <p className="text-sm text-ink/70">{profile.departmentName}</p>
                <div className="flex flex-wrap gap-2">
                  {profile.skills.length === 0 ? (
                    <span className="rounded-full bg-ink/5 px-3 py-1 text-xs text-ink/50">No skills yet</span>
                  ) : (
                    profile.skills.map((skill) => (
                      <span key={skill} className="rounded-full bg-ink/5 px-3 py-1 text-xs text-ink/70">
                        {skill}
                      </span>
                    ))
                  )}
                </div>
                <button
                  type="button"
                  className="h-fit rounded-full bg-ocean px-4 py-2 text-sm font-semibold text-bone transition hover:bg-ocean/90"
                  onClick={() => handleOpenDialog(profile)}
                >
                  Upload CV
                </button>
              </div>
            ))
          )}
        </div>
        <div className="flex flex-col gap-4 border-t border-ink/10 px-6 py-4 sm:flex-row sm:items-center sm:justify-between">
          <p className="text-sm text-ink/60">
            Showing {pagedProfiles.length} of {sortedProfiles.length} profiles
          </p>
          <div className="flex items-center gap-2">
            <button
              type="button"
              className="rounded-full border border-ink/20 px-3 py-1 text-sm text-ink/70 disabled:opacity-40"
              onClick={() => setPage((prev) => Math.max(1, prev - 1))}
              disabled={currentPage === 1}
            >
              Prev
            </button>
            {Array.from({ length: pageCount }, (_, index) => index + 1).map((pageNumber) => (
              <button
                key={pageNumber}
                type="button"
                className={`h-8 w-8 rounded-full text-sm font-semibold ${pageNumber === currentPage ? 'bg-ink text-bone' : 'text-ink/70'}`}
                onClick={() => setPage(pageNumber)}
              >
                {pageNumber}
              </button>
            ))}
            <button
              type="button"
              className="rounded-full border border-ink/20 px-3 py-1 text-sm text-ink/70 disabled:opacity-40"
              onClick={() => setPage((prev) => Math.min(pageCount, prev + 1))}
              disabled={currentPage === pageCount}
            >
              Next
            </button>
          </div>
        </div>
      </section>

      <CvUploadDialog
        isOpen={isDialogOpen}
        profile={selectedProfile}
        onClose={() => setIsDialogOpen(false)}
        onProfileUpdated={handleProfileUpdated}
      />
    </div>
  )
}
