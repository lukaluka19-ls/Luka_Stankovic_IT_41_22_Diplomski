import { useState } from 'react'
import { AvailabilityCalendar } from '../features/reservations/components/AvailabilityCalendar'
import { ReservationForm } from '../features/reservations/components/ReservationForm'

export function BookingPage() {
  const [selectedDate, setSelectedDate] = useState<string | null>(null)

  return (
    <div className="mx-auto max-w-5xl px-4 py-10 sm:px-6">
      <h1 className="mb-2 text-3xl font-black text-ink">Zakazivanje proslave</h1>
      <p className="mb-8 text-ink-soft">
        Izaberite slobodan datum, popunite podatke o proslavi i potvrdite rezervaciju. Termin se privremeno
        zakljucava do fizicke potvrde u igraonici.
      </p>

      <div className="grid grid-cols-1 gap-8 lg:grid-cols-2">
        <AvailabilityCalendar selectedDate={selectedDate} onSelectDate={setSelectedDate} />
        <ReservationForm selectedDate={selectedDate} />
      </div>
    </div>
  )
}
