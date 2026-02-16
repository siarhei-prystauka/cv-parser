import { useMemo, useState } from 'react'
import type { TaxonomySkill } from '../types/settings'

type TaxonomyViewerProps = {
  skills: TaxonomySkill[]
}

type GroupedSkills = Record<string, TaxonomySkill[]>

export const TaxonomyViewer = ({ skills }: TaxonomyViewerProps) => {
  const [searchQuery, setSearchQuery] = useState('')
  const [expandedCategories, setExpandedCategories] = useState<Set<string>>(new Set())

  const groupedSkills = useMemo(() => {
    const filtered = searchQuery.trim()
      ? skills.filter((skill) => {
          const query = searchQuery.toLowerCase()
          return (
            skill.name.toLowerCase().includes(query) ||
            skill.category.toLowerCase().includes(query) ||
            skill.aliases.some((alias) => alias.toLowerCase().includes(query))
          )
        })
      : skills

    const grouped: GroupedSkills = {}
    for (const skill of filtered) {
      if (!grouped[skill.category]) {
        grouped[skill.category] = []
      }
      grouped[skill.category].push(skill)
    }

    return grouped
  }, [skills, searchQuery])

  const categories = useMemo(() => Object.keys(groupedSkills).sort(), [groupedSkills])

  const toggleCategory = (category: string) => {
    setExpandedCategories((prev) => {
      const next = new Set(prev)
      if (next.has(category)) {
        next.delete(category)
      } else {
        next.add(category)
      }
      return next
    })
  }

  const expandAll = () => {
    setExpandedCategories(new Set(categories))
  }

  const collapseAll = () => {
    setExpandedCategories(new Set())
  }

  const totalSkills = useMemo(
    () => Object.values(groupedSkills).reduce((sum, categorySkills) => sum + categorySkills.length, 0),
    [groupedSkills]
  )

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <input
          type="search"
          placeholder="Search skills, categories, or aliases..."
          value={searchQuery}
          onChange={(event) => setSearchQuery(event.target.value)}
          className="w-full rounded-full border border-ink/20 bg-white px-4 py-2 text-sm text-ink shadow-sm focus:border-ocean focus:outline-none focus:ring-2 focus:ring-ocean/20 sm:w-80"
        />
        <div className="flex items-center gap-2">
          <button
            type="button"
            onClick={expandAll}
            className="rounded-full border border-ink/20 px-3 py-1 text-xs font-semibold text-ink/70 transition hover:border-ink/40"
          >
            Expand All
          </button>
          <button
            type="button"
            onClick={collapseAll}
            className="rounded-full border border-ink/20 px-3 py-1 text-xs font-semibold text-ink/70 transition hover:border-ink/40"
          >
            Collapse All
          </button>
        </div>
      </div>

      <div className="rounded-2xl border border-ink/10 bg-white/70 p-4">
        <p className="text-xs text-ink/60">
          {totalSkills} {totalSkills === 1 ? 'skill' : 'skills'} across {categories.length}{' '}
          {categories.length === 1 ? 'category' : 'categories'}
        </p>
      </div>

      {categories.length === 0 ? (
        <div className="rounded-2xl border border-ink/10 bg-white/70 p-8 text-center">
          <p className="text-sm text-ink/60">No skills found matching your search.</p>
        </div>
      ) : (
        <div className="space-y-3">
          {categories.map((category) => {
            const categorySkills = groupedSkills[category]
            const isExpanded = expandedCategories.has(category)

            return (
              <div key={category} className="rounded-2xl border border-ink/10 bg-white/70 shadow-sm">
                <button
                  type="button"
                  onClick={() => toggleCategory(category)}
                  className="flex w-full items-center justify-between p-4 text-left transition hover:bg-sand/30"
                >
                  <div className="flex items-center gap-3">
                    <span
                      className={`text-ink/40 transition-transform ${
                        isExpanded ? 'rotate-90' : 'rotate-0'
                      }`}
                    >
                      ▶
                    </span>
                    <h3 className="text-sm font-semibold text-ink">{category}</h3>
                    <span className="rounded-full bg-ink/10 px-2 py-0.5 text-xs text-ink/60">
                      {categorySkills.length}
                    </span>
                  </div>
                </button>

                {isExpanded && (
                  <div className="border-t border-ink/10 p-4">
                    <div className="space-y-3">
                      {categorySkills.map((skill) => (
                        <div
                          key={skill.name}
                          className="rounded-xl border border-ink/10 bg-white p-3"
                        >
                          <p className="text-sm font-medium text-ink">{skill.name}</p>
                          {skill.aliases.length > 0 && (
                            <div className="mt-2 flex flex-wrap gap-1.5">
                              {skill.aliases.map((alias) => (
                                <span
                                  key={alias}
                                  className="rounded-full bg-sand/50 px-2 py-0.5 text-xs text-ink/70"
                                >
                                  {alias}
                                </span>
                              ))}
                            </div>
                          )}
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            )
          })}
        </div>
      )}
    </div>
  )
}
