import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { TaxonomyViewer } from '../TaxonomyViewer'
import type { TaxonomySkill } from '../../types/settings'

describe('TaxonomyViewer', () => {
  const mockSkills: TaxonomySkill[] = [
    {
      name: 'JavaScript',
      category: 'Programming Languages',
      aliases: ['JS', 'ECMAScript'],
    },
    {
      name: 'TypeScript',
      category: 'Programming Languages',
      aliases: ['TS'],
    },
    {
      name: 'React',
      category: 'Frontend Frameworks',
      aliases: ['React.js', 'ReactJS'],
    },
    {
      name: 'Vue',
      category: 'Frontend Frameworks',
      aliases: ['Vue.js', 'VueJS'],
    },
    {
      name: 'Docker',
      category: 'DevOps Tools',
      aliases: ['Container'],
    },
  ]

  it('render - with skills - displays skills grouped by category', () => {
    render(<TaxonomyViewer skills={mockSkills} />)

    expect(screen.getByText('Programming Languages')).toBeInTheDocument()
    expect(screen.getByText('Frontend Frameworks')).toBeInTheDocument()
    expect(screen.getByText('DevOps Tools')).toBeInTheDocument()
  })

  it('render - with skills - displays correct skill count per category', () => {
    render(<TaxonomyViewer skills={mockSkills} />)

    const programmingLanguagesButton = screen.getByRole('button', {
      name: /programming languages/i,
    })
    expect(programmingLanguagesButton).toHaveTextContent('2')

    const frontendFrameworksButton = screen.getByRole('button', {
      name: /frontend frameworks/i,
    })
    expect(frontendFrameworksButton).toHaveTextContent('2')

    const devopsToolsButton = screen.getByRole('button', { name: /devops tools/i })
    expect(devopsToolsButton).toHaveTextContent('1')
  })

  it('render - with skills - displays total skills count', () => {
    render(<TaxonomyViewer skills={mockSkills} />)

    expect(screen.getByText(/5 skills across 3 categories/i)).toBeInTheDocument()
  })

  it('render - initially - all categories are collapsed', () => {
    render(<TaxonomyViewer skills={mockSkills} />)

    expect(screen.queryByText('JavaScript')).not.toBeInTheDocument()
    expect(screen.queryByText('TypeScript')).not.toBeInTheDocument()
    expect(screen.queryByText('React')).not.toBeInTheDocument()
  })

  it('click - on category header - expands category and shows skills', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const categoryButton = screen.getByRole('button', { name: /programming languages/i })
    await user.click(categoryButton)

    expect(screen.getByText('JavaScript')).toBeInTheDocument()
    expect(screen.getByText('TypeScript')).toBeInTheDocument()
  })

  it('click - on expanded category - collapses category and hides skills', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const categoryButton = screen.getByRole('button', { name: /programming languages/i })
    await user.click(categoryButton)

    expect(screen.getByText('JavaScript')).toBeInTheDocument()

    await user.click(categoryButton)

    expect(screen.queryByText('JavaScript')).not.toBeInTheDocument()
  })

  it('click - expand all button - expands all categories', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const expandAllButton = screen.getByRole('button', { name: /expand all/i })
    await user.click(expandAllButton)

    expect(screen.getByText('JavaScript')).toBeInTheDocument()
    expect(screen.getByText('TypeScript')).toBeInTheDocument()
    expect(screen.getByText('React')).toBeInTheDocument()
    expect(screen.getByText('Vue')).toBeInTheDocument()
    expect(screen.getByText('Docker')).toBeInTheDocument()
  })

  it('click - collapse all button - collapses all categories', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const expandAllButton = screen.getByRole('button', { name: /expand all/i })
    await user.click(expandAllButton)

    expect(screen.getByText('JavaScript')).toBeInTheDocument()

    const collapseAllButton = screen.getByRole('button', { name: /collapse all/i })
    await user.click(collapseAllButton)

    expect(screen.queryByText('JavaScript')).not.toBeInTheDocument()
    expect(screen.queryByText('React')).not.toBeInTheDocument()
  })

  it('render - expanded category - displays skill aliases correctly', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const categoryButton = screen.getByRole('button', { name: /programming languages/i })
    await user.click(categoryButton)

    expect(screen.getByText('JS')).toBeInTheDocument()
    expect(screen.getByText('ECMAScript')).toBeInTheDocument()
    expect(screen.getByText('TS')).toBeInTheDocument()
  })

  it('search - by skill name - filters skills correctly', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const searchInput = screen.getByPlaceholderText(/search skills, categories, or aliases/i)
    await user.type(searchInput, 'JavaScript')

    expect(screen.getByText('Programming Languages')).toBeInTheDocument()
    expect(screen.queryByText('Frontend Frameworks')).not.toBeInTheDocument()
    expect(screen.queryByText('DevOps Tools')).not.toBeInTheDocument()
    expect(screen.getByText(/1 skill across 1 category/i)).toBeInTheDocument()
  })

  it('search - by category name - filters categories correctly', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const searchInput = screen.getByPlaceholderText(/search skills, categories, or aliases/i)
    await user.type(searchInput, 'Frontend')

    expect(screen.queryByText('Programming Languages')).not.toBeInTheDocument()
    expect(screen.getByText('Frontend Frameworks')).toBeInTheDocument()
    expect(screen.queryByText('DevOps Tools')).not.toBeInTheDocument()
    expect(screen.getByText(/2 skills across 1 category/i)).toBeInTheDocument()
  })

  it('search - by alias - filters skills with matching alias', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const searchInput = screen.getByPlaceholderText(/search skills, categories, or aliases/i)
    await user.type(searchInput, 'ReactJS')

    expect(screen.queryByText('Programming Languages')).not.toBeInTheDocument()
    expect(screen.getByText('Frontend Frameworks')).toBeInTheDocument()
    expect(screen.queryByText('DevOps Tools')).not.toBeInTheDocument()
    expect(screen.getByText(/1 skill across 1 category/i)).toBeInTheDocument()
  })

  it('search - with no matches - displays no results message', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const searchInput = screen.getByPlaceholderText(/search skills, categories, or aliases/i)
    await user.type(searchInput, 'NonexistentSkill')

    expect(screen.getByText('No skills found matching your search.')).toBeInTheDocument()
    expect(screen.getByText(/0 skills across 0 categories/i)).toBeInTheDocument()
  })

  it('search - case insensitive - filters skills regardless of case', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const searchInput = screen.getByPlaceholderText(/search skills, categories, or aliases/i)
    await user.type(searchInput, 'javascript')

    expect(screen.getByText('Programming Languages')).toBeInTheDocument()
    expect(screen.getByText(/1 skill across 1 category/i)).toBeInTheDocument()
  })

  it('search - partial match - filters skills with partial name match', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const searchInput = screen.getByPlaceholderText(/search skills, categories, or aliases/i)
    await user.type(searchInput, 'Script')

    expect(screen.getByText('Programming Languages')).toBeInTheDocument()
    expect(screen.getByText(/2 skills across 1 category/i)).toBeInTheDocument()
  })

  it('search - then expand category - displays filtered skills with aliases', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const searchInput = screen.getByPlaceholderText(/search skills, categories, or aliases/i)
    await user.type(searchInput, 'React')

    const categoryButton = screen.getByRole('button', { name: /frontend frameworks/i })
    await user.click(categoryButton)

    expect(screen.getByText('React')).toBeInTheDocument()
    expect(screen.getByText('React.js')).toBeInTheDocument()
    expect(screen.getByText('ReactJS')).toBeInTheDocument()
  })

  it('render - with empty skills array - displays no results', () => {
    render(<TaxonomyViewer skills={[]} />)

    expect(screen.getByText(/0 skills across 0 categories/i)).toBeInTheDocument()
    expect(screen.getByText('No skills found matching your search.')).toBeInTheDocument()
  })

  it('render - skill without aliases - does not display aliases section', async () => {
    const user = userEvent.setup()
    const skillsWithoutAliases: TaxonomySkill[] = [
      {
        name: 'Python',
        category: 'Programming Languages',
        aliases: [],
      },
    ]
    render(<TaxonomyViewer skills={skillsWithoutAliases} />)

    const categoryButton = screen.getByRole('button', { name: /programming languages/i })
    await user.click(categoryButton)

    expect(screen.getByText('Python')).toBeInTheDocument()
    const pythonCard = screen.getByText('Python').closest('div')
    expect(pythonCard).not.toHaveTextContent('aliases')
  })

  it('render - single skill - displays singular form in count', () => {
    const singleSkill: TaxonomySkill[] = [
      {
        name: 'Python',
        category: 'Programming Languages',
        aliases: ['py'],
      },
    ]
    render(<TaxonomyViewer skills={singleSkill} />)

    expect(screen.getByText(/1 skill across 1 category/i)).toBeInTheDocument()
  })

  it('search - clear search input - restores all skills', async () => {
    const user = userEvent.setup()
    render(<TaxonomyViewer skills={mockSkills} />)

    const searchInput = screen.getByPlaceholderText(/search skills, categories, or aliases/i)
    await user.type(searchInput, 'JavaScript')

    expect(screen.getByText(/1 skill across 1 category/i)).toBeInTheDocument()

    await user.clear(searchInput)

    expect(screen.getByText(/5 skills across 3 categories/i)).toBeInTheDocument()
  })
})
