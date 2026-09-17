export interface GalleryPhoto {
  src: string
  alt: string
  dateLabel: string
  source: string
}

// Fotografije stvarnog prostora igraonice JoyLand, preuzete sa njihovog
// Google poslovnog profila (zvanicne fotografije i fotografije iz recenzija
// korisnika) i njihovog oglasa na sajtu igraland.rs. Poredjane od najnovije
// ka najstarijoj, prema datumima dostupnim na izvoru.
export const GALLERY_PHOTOS: GalleryPhoto[] = [
  { src: '/gallery/g01.webp', alt: 'Prostor za sedenje i druzenje', dateLabel: 'Maj 2026.', source: 'igraland.rs' },
  { src: '/gallery/g02.webp', alt: 'Radionica za decu', dateLabel: 'Maj 2026.', source: 'igraland.rs' },
  { src: '/gallery/g03.webp', alt: 'Aparat sa plisanim igrackama', dateLabel: 'Maj 2026.', source: 'igraland.rs' },
  { src: '/gallery/g04.webp', alt: 'Soba za igrice', dateLabel: 'Maj 2026.', source: 'igraland.rs' },
  { src: '/gallery/g05.webp', alt: 'Zidne slike na stepenistu', dateLabel: 'Maj 2026.', source: 'igraland.rs' },
  { src: '/gallery/g06.webp', alt: 'Tobogan u igraonici', dateLabel: 'Maj 2026.', source: 'igraland.rs' },
  { src: '/gallery/g07.jpg', alt: 'Druzenje uz neonsko osvetljenje', dateLabel: 'Februar 2026.', source: 'Google poslovni profil' },
  { src: '/gallery/g08.jpg', alt: 'Prostor za sedenje', dateLabel: 'Februar 2026.', source: 'Google poslovni profil' },
  { src: '/gallery/g09.jpg', alt: 'Igra fudbala na stolu', dateLabel: 'Decembar 2025.', source: 'Google recenzija' },
  { src: '/gallery/g10.jpg', alt: 'Aparat za hvatanje igracaka', dateLabel: 'Decembar 2025.', source: 'Google recenzija' },
  { src: '/gallery/g11.jpg', alt: 'Interaktivna soba sa projekcijom', dateLabel: 'Decembar 2025.', source: 'Google recenzija' },
  { src: '/gallery/g12.jpg', alt: 'Stepeniste sa zidnim slikama', dateLabel: 'Decembar 2025.', source: 'Google recenzija' },
  { src: '/gallery/g13.jpg', alt: 'Zidna dekoracija', dateLabel: 'Decembar 2025.', source: 'Google recenzija' },
  { src: '/gallery/g14.jpg', alt: 'Razbijanje pinjate', dateLabel: 'Decembar 2025.', source: 'Google recenzija' },
  { src: '/gallery/g15.jpg', alt: 'Grupna fotografija sa rodjendana', dateLabel: 'Decembar 2025.', source: 'Google recenzija' },
  { src: '/gallery/g16.jpg', alt: 'Elektronska pikado tabla', dateLabel: 'Decembar 2025.', source: 'Google recenzija' },
  { src: '/gallery/g17.jpg', alt: 'Druzenje sa golubovima', dateLabel: 'Decembar 2025.', source: 'Google recenzija' },
  { src: '/gallery/g18.jpg', alt: 'Radionica za decu', dateLabel: 'Decembar 2025.', source: 'Google recenzija' },
  { src: '/gallery/g19.jpg', alt: 'Dekoracija za rodjendan', dateLabel: 'Decembar 2025.', source: 'Google recenzija' },
]
