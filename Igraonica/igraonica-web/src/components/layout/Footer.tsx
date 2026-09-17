import { BUSINESS } from '../../lib/constants'

export function Footer() {
  return (
    <footer className="border-t border-border bg-white">
      <div className="mx-auto flex max-w-6xl flex-col gap-2 px-4 py-8 text-sm text-ink-soft sm:px-6">
        <p className="font-semibold text-ink">{BUSINESS.fullName}</p>
        <p>{BUSINESS.address}</p>
        <p>
          {BUSINESS.phone} &middot; {BUSINESS.email}
        </p>
        <p className="mt-4 text-xs">
          &copy; {new Date().getFullYear()} {BUSINESS.name}. Sistem za rezervaciju termina proslava.
        </p>
      </div>
    </footer>
  )
}
