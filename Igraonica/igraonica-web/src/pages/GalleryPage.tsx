import { useState } from 'react'
import { GALLERY_PHOTOS } from '../lib/galleryPhotos'

export function GalleryPage() {
  const [openIndex, setOpenIndex] = useState<number | null>(null)

  return (
    <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6">
      <h1 className="mb-2 text-3xl font-black text-ink">Galerija</h1>
      <p className="mb-8 text-ink-soft">
        Fotografije prostora igraonice JoyLand, preuzete sa naseg Google poslovnog profila i sajta igraland.rs,
        poredjane od najnovije ka najstarijoj.
      </p>

      <div className="columns-2 gap-3 sm:columns-3 lg:columns-4 [&>*]:mb-3">
        {GALLERY_PHOTOS.map((photo, i) => (
          <button
            key={photo.src}
            type="button"
            onClick={() => setOpenIndex(i)}
            className="block w-full overflow-hidden rounded-xl border border-border bg-white shadow-sm transition-transform hover:-translate-y-0.5 hover:shadow-md"
          >
            <img src={photo.src} alt={photo.alt} loading="lazy" className="w-full object-cover" />
          </button>
        ))}
      </div>

      {openIndex !== null && (
        <Lightbox
          photos={GALLERY_PHOTOS}
          index={openIndex}
          onClose={() => setOpenIndex(null)}
          onNavigate={setOpenIndex}
        />
      )}
    </div>
  )
}

function Lightbox({
  photos,
  index,
  onClose,
  onNavigate,
}: {
  photos: typeof GALLERY_PHOTOS
  index: number
  onClose: () => void
  onNavigate: (i: number) => void
}) {
  const photo = photos[index]

  return (
    <div
      className="fixed inset-0 z-50 flex flex-col items-center justify-center gap-4 bg-navy/95 p-4"
      onClick={onClose}
    >
      <button
        type="button"
        onClick={onClose}
        className="absolute top-4 right-4 rounded-full bg-white/10 px-3 py-1.5 text-sm font-semibold text-white hover:bg-white/20"
      >
        Zatvori &times;
      </button>

      <button
        type="button"
        onClick={(e) => {
          e.stopPropagation()
          onNavigate((index - 1 + photos.length) % photos.length)
        }}
        className="absolute left-4 top-1/2 -translate-y-1/2 rounded-full bg-white/10 px-4 py-3 text-xl text-white hover:bg-white/20"
        aria-label="Prethodna fotografija"
      >
        &larr;
      </button>

      <img
        src={photo.src}
        alt={photo.alt}
        onClick={(e) => e.stopPropagation()}
        className="max-h-[80vh] max-w-full rounded-lg object-contain shadow-2xl"
      />

      <div className="text-center text-sm text-white/70">
        {photo.alt} &middot; {photo.dateLabel} &middot; {photo.source}
      </div>

      <button
        type="button"
        onClick={(e) => {
          e.stopPropagation()
          onNavigate((index + 1) % photos.length)
        }}
        className="absolute right-4 top-1/2 -translate-y-1/2 rounded-full bg-white/10 px-4 py-3 text-xl text-white hover:bg-white/20"
        aria-label="Sledeca fotografija"
      >
        &rarr;
      </button>
    </div>
  )
}
