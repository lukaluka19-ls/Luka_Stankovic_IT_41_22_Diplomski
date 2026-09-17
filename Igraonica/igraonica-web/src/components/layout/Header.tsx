import { useState } from 'react'
import { Link, NavLink, useNavigate } from 'react-router-dom'
import { useAuthStore } from '../../lib/authStore'
import { Button } from '../ui/Button'
import { BUSINESS } from '../../lib/constants'

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  `text-sm font-semibold transition-colors hover:text-green ${isActive ? 'text-green' : 'text-white/85'}`

const mobileNavLinkClass = ({ isActive }: { isActive: boolean }) =>
  `block rounded-lg px-3 py-2.5 text-base font-semibold transition-colors ${
    isActive ? 'bg-white/10 text-green' : 'text-white/85 hover:bg-white/5'
  }`

export function Header() {
  const { user, logout } = useAuthStore()
  const navigate = useNavigate()
  const [menuOpen, setMenuOpen] = useState(false)

  const closeMenu = () => setMenuOpen(false)

  return (
    <header className="sticky top-0 z-30 bg-navy/95 shadow-lg shadow-black/20 backdrop-blur">
      <div className="mx-auto flex max-w-6xl items-center justify-between px-4 py-1.5 sm:px-6">
        <Link to="/" className="flex items-center gap-2.5 sm:gap-3" onClick={closeMenu}>
          <img src="/logo.svg" alt={BUSINESS.name} className="h-12 w-12 drop-shadow-lg sm:h-20 sm:w-20" />
          <span className="text-xl font-black tracking-tight text-white sm:text-2xl">{BUSINESS.name}</span>
        </Link>

        <nav className="hidden items-center gap-6 sm:flex">
          <NavLink to="/" end className={navLinkClass}>
            Pocetna
          </NavLink>
          <NavLink to="/zakazivanje" className={navLinkClass}>
            Zakazivanje
          </NavLink>
          <NavLink to="/galerija" className={navLinkClass}>
            Galerija
          </NavLink>
          {user?.role === 'Client' && (
            <NavLink to="/moje-rezervacije" className={navLinkClass}>
              Moje rezervacije
            </NavLink>
          )}
          {user?.role === 'Administrator' && (
            <NavLink to="/admin" className={navLinkClass}>
              Administracija
            </NavLink>
          )}
        </nav>

        <div className="hidden items-center gap-3 sm:flex">
          {user ? (
            <>
              <span className="text-sm text-white/70">Zdravo, {user.firstName}</span>
              <Button
                variant="ghost-dark"
                onClick={() => {
                  logout()
                  navigate('/')
                }}
              >
                Odjava
              </Button>
            </>
          ) : (
            <>
              <Button variant="ghost-dark" onClick={() => navigate('/prijava')}>
                Prijava
              </Button>
              <Button onClick={() => navigate('/registracija')}>Registracija</Button>
            </>
          )}
        </div>

        <button
          type="button"
          onClick={() => setMenuOpen((v) => !v)}
          className="flex h-10 w-10 items-center justify-center rounded-lg text-white sm:hidden"
          aria-label="Meni"
          aria-expanded={menuOpen}
        >
          {menuOpen ? (
            <svg viewBox="0 0 24 24" className="h-6 w-6" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M6 6l12 12M18 6L6 18" strokeLinecap="round" />
            </svg>
          ) : (
            <svg viewBox="0 0 24 24" className="h-6 w-6" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M4 7h16M4 12h16M4 17h16" strokeLinecap="round" />
            </svg>
          )}
        </button>
      </div>

      {menuOpen && (
        <div className="border-t border-white/10 bg-navy px-4 pb-4 sm:hidden">
          <nav className="flex flex-col gap-1 pt-2">
            <NavLink to="/" end className={mobileNavLinkClass} onClick={closeMenu}>
              Pocetna
            </NavLink>
            <NavLink to="/zakazivanje" className={mobileNavLinkClass} onClick={closeMenu}>
              Zakazivanje
            </NavLink>
            <NavLink to="/galerija" className={mobileNavLinkClass} onClick={closeMenu}>
              Galerija
            </NavLink>
            {user?.role === 'Client' && (
              <NavLink to="/moje-rezervacije" className={mobileNavLinkClass} onClick={closeMenu}>
                Moje rezervacije
              </NavLink>
            )}
            {user?.role === 'Administrator' && (
              <NavLink to="/admin" className={mobileNavLinkClass} onClick={closeMenu}>
                Administracija
              </NavLink>
            )}
          </nav>

          <div className="mt-3 flex flex-col gap-2 border-t border-white/10 pt-3">
            {user ? (
              <>
                <span className="px-3 text-sm text-white/70">Zdravo, {user.firstName}</span>
                <Button
                  variant="ghost-dark"
                  onClick={() => {
                    logout()
                    closeMenu()
                    navigate('/')
                  }}
                >
                  Odjava
                </Button>
              </>
            ) : (
              <>
                <Button
                  variant="ghost-dark"
                  onClick={() => {
                    closeMenu()
                    navigate('/prijava')
                  }}
                >
                  Prijava
                </Button>
                <Button
                  onClick={() => {
                    closeMenu()
                    navigate('/registracija')
                  }}
                >
                  Registracija
                </Button>
              </>
            )}
          </div>
        </div>
      )}
    </header>
  )
}
