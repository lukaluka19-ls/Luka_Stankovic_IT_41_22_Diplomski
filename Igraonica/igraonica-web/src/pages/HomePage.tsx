import { useNavigate } from 'react-router-dom'
import { PackagesSection } from '../features/packages/PackagesSection'
import { HeroCarousel } from '../components/ui/HeroCarousel'
import { Button } from '../components/ui/Button'
import { BUSINESS } from '../lib/constants'
import { useAuthStore } from '../lib/authStore'

export function HomePage() {
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)

  return (
    <div className="flex flex-col gap-16 pb-16">
      <section className="relative isolate overflow-hidden bg-navy px-4 pt-16 pb-16 text-center sm:px-6 sm:pt-20 sm:pb-24">
        <HeroCarousel />

        <div className="relative z-10">
          <span className="mb-3 inline-flex items-center gap-2 rounded-full bg-white/10 px-4 py-1.5 text-xs font-bold text-green shadow-sm">
            <span className="h-1.5 w-1.5 animate-pulse rounded-full bg-green" />
            {BUSINESS.tagline}
          </span>
          <h1 className="mx-auto max-w-2xl text-4xl leading-tight font-black tracking-tight text-white sm:text-5xl">
            Nezaboravan rodjendan u igraonici{' '}
            <span className="bg-gradient-to-r from-green to-cyan bg-clip-text text-transparent">
              {BUSINESS.name}
            </span>
          </h1>
          <p className="mx-auto mt-4 max-w-xl text-white/70">
            Proverite dostupne termine i rezervisite proslavu za svoje dete u nekoliko klikova &mdash; bez telefonskih
            poziva i cekanja na odgovor.
          </p>
          <div className="mt-8 flex flex-col justify-center gap-3 sm:flex-row">
            <Button onClick={() => navigate('/zakazivanje')}>Zakazi termin</Button>
            {!user && (
              <Button variant="ghost-dark" onClick={() => navigate('/registracija')}>
                Napravi nalog
              </Button>
            )}
          </div>
        </div>
      </section>

      <section className="mx-auto w-full max-w-6xl px-4 sm:px-6">
        <h2 className="mb-6 text-center text-2xl font-bold text-ink">Ponuda paketa</h2>
        <PackagesSection />
      </section>
    </div>
  )
}
