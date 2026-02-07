import { NavLink, Outlet } from 'react-router-dom'

const navLinkClasses = ({ isActive }: { isActive: boolean }): string =>
  `text-sm font-medium tracking-wide transition ${isActive ? 'text-ink' : 'text-ink/60 hover:text-ink'}`

export const MainLayout = () => (
  <div className="min-h-screen">
    <header className="sticky top-0 z-20 border-b border-ink/10 bg-bone/80 backdrop-blur">
      <div className="mx-auto flex w-full max-w-6xl items-center justify-between px-6 py-4">
        <div className="flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center rounded-2xl bg-ocean text-white shadow-glow">
            <span className="font-display text-lg">CV</span>
          </div>
          <div>
            <p className="font-display text-lg font-semibold text-ink">Onboard Studio</p>
            <p className="text-xs uppercase tracking-[0.3em] text-ink/45">Newcomer profiles</p>
          </div>
        </div>
        <nav className="flex items-center gap-6">
          <NavLink to="/" className={navLinkClasses} end>
            Main
          </NavLink>
          <NavLink to="/admin" className={navLinkClasses}>
            Admin
          </NavLink>
        </nav>
      </div>
    </header>
    <main className="mx-auto w-full max-w-6xl px-6 pb-16 pt-10">
      <Outlet />
    </main>
  </div>
)
