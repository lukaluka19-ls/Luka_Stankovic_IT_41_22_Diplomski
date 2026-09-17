import { useEffect, useState } from 'react'
import clsx from 'clsx'

// Najbolji, najnoviji kadrovi iz galerije - koriste se u velikoj rezoluciji
// za pozadinu pocetne strane.
const PHOTOS = [
  '/gallery/g06.webp',
  '/gallery/g07.jpg',
  '/gallery/g14.jpg',
  '/gallery/g10.jpg',
  '/gallery/g15.jpg',
  '/gallery/g17.jpg',
  '/gallery/g12.jpg',
  '/gallery/g19.jpg',
]

const INTERVAL_MS = 5000

export function HeroCarousel() {
  const [active, setActive] = useState(0)

  useEffect(() => {
    const timer = setInterval(() => setActive((i) => (i + 1) % PHOTOS.length), INTERVAL_MS)
    return () => clearInterval(timer)
  }, [])

  return (
    <div className="absolute inset-0 overflow-hidden">
      {PHOTOS.map((src, i) => (
        <img
          key={src}
          src={src}
          alt=""
          className={clsx(
            'absolute inset-0 h-full w-full object-cover transition-opacity duration-1000 ease-in-out',
            i === active ? 'opacity-100' : 'opacity-0',
          )}
        />
      ))}
      {/* Teget preliv radi citljivosti teksta preko fotografija, u skladu sa bojama loga. */}
      <div className="absolute inset-0 bg-gradient-to-b from-navy/85 via-navy/80 to-navy" />

      <div className="absolute bottom-5 left-1/2 flex -translate-x-1/2 gap-2">
        {PHOTOS.map((src, i) => (
          <button
            key={src}
            type="button"
            aria-label={`Prikazi fotografiju ${i + 1}`}
            onClick={() => setActive(i)}
            className={clsx(
              'h-1.5 rounded-full transition-all',
              i === active ? 'w-6 bg-green' : 'w-1.5 bg-white/40 hover:bg-white/60',
            )}
          />
        ))}
      </div>
    </div>
  )
}
