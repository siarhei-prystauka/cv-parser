import { NavLink, Outlet } from 'react-router-dom'

const navLinkClasses = ({ isActive }: { isActive: boolean }): string =>
  `text-sm font-medium tracking-wide transition ${isActive ? 'text-ink' : 'text-ink/60 hover:text-ink'}`

export const AdminLayout = () => (
  <div className="min-h-screen bg-ink text-bone">
    <header className="border-b border-bone/10">
      <div className="mx-auto flex w-full max-w-6xl items-center justify-between px-6 py-4">
        <div>
          <p className="font-display text-lg font-semibold">Admin Console</p>
          <p className="text-xs uppercase tracking-[0.3em] text-bone/60">Future configuration</p>
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
