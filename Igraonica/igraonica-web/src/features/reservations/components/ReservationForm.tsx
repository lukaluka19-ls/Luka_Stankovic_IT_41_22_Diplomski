import { useMemo, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { packagesApi, timeSlotsApi } from '../../../api/endpoints/catalog'
import { useCreateReservation } from '../hooks/useCreateReservation'
import { useAuthStore } from '../../../lib/authStore'
import { formatDate, formatPrice, formatPackageType } from '../../../lib/formatters'
import { Button } from '../../../components/ui/Button'
import { Input } from '../../../components/ui/Input'
import { Card } from '../../../components/ui/Card'
import clsx from 'clsx'

interface ReservationFormProps {
  selectedDate: string | null
}

export function ReservationForm({ selectedDate }: ReservationFormProps) {
  const user = useAuthStore((s) => s.user)
  const { data: packages } = useQuery({ queryKey: ['packages'], queryFn: packagesApi.getActive })
  const { data: timeSlots } = useQuery({ queryKey: ['timeslots'], queryFn: timeSlotsApi.getActive })
  const createReservation = useCreateReservation()

  const [timeSlotId, setTimeSlotId] = useState('')
  const [packageId, setPackageId] = useState('')
  const [childName, setChildName] = useState('')
  const [childAge, setChildAge] = useState(6)
  const [numberOfChildren, setNumberOfChildren] = useState(10)

  const selectedPackage = packages?.find((p) => p.id === packageId)
  const estimatedPrice = useMemo(() => {
    if (!selectedPackage) return null
    return Number(selectedPackage.basePrice) + Number(selectedPackage.pricePerChild) * numberOfChildren
  }, [selectedPackage, numberOfChildren])

  if (!user) {
    return (
      <Card>
        <p className="text-sm text-ink-soft">
          Da biste napravili rezervaciju, prijavite se na svoj nalog. Ako nemate nalog, registrujte se i potvrdite
          adresu elektronske poste.
        </p>
      </Card>
    )
  }

  if (!selectedDate) {
    return (
      <Card>
        <p className="text-sm text-ink-soft">Izaberite slobodan datum u kalendaru da biste nastavili.</p>
      </Card>
    )
  }

  const canSubmit = timeSlotId && packageId && childName.trim().length > 0 && !createReservation.isPending

  return (
    <Card>
      <h3 className="mb-1 text-lg font-bold text-ink">Obrazac za kreiranje rezervacije</h3>
      <p className="mb-4 text-sm text-ink-soft">Izabran datum: {formatDate(selectedDate)}</p>

      <form
        className="flex flex-col gap-4"
        onSubmit={(e) => {
          e.preventDefault()
          if (!canSubmit) return
          createReservation.mutate({
            childName,
            childAge,
            eventDate: selectedDate,
            timeSlotId,
            numberOfChildren,
            packageId,
          })
        }}
      >
        <div>
          <span className="mb-1.5 block text-sm font-medium text-ink">Termin</span>
          <div className="flex flex-wrap gap-2">
            {timeSlots?.map((slot) => (
              <button
                key={slot.id}
                type="button"
                onClick={() => setTimeSlotId(slot.id)}
                className={clsx(
                  'rounded-full border px-4 py-2 text-sm font-semibold transition-colors',
                  timeSlotId === slot.id
                    ? 'border-purple bg-purple text-white'
                    : 'border-border bg-white text-ink hover:border-purple',
                )}
              >
                {slot.name} ({slot.startTime.slice(0, 5)}&ndash;{slot.endTime.slice(0, 5)})
              </button>
            ))}
          </div>
        </div>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <Input
            label="Ime deteta"
            value={childName}
            onChange={(e) => setChildName(e.target.value)}
            placeholder="npr. Marko"
            required
          />
          <Input
            label="Uzrast deteta"
            type="number"
            min={1}
            max={14}
            value={childAge}
            onChange={(e) => setChildAge(Number(e.target.value))}
            required
          />
        </div>

        <Input
          label="Ocekivan broj dece"
          type="number"
          min={1}
          max={40}
          value={numberOfChildren}
          onChange={(e) => setNumberOfChildren(Number(e.target.value))}
          required
        />

        <div>
          <span className="mb-1.5 block text-sm font-medium text-ink">Paket</span>
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
            {packages?.map((pkg) => (
              <button
                key={pkg.id}
                type="button"
                onClick={() => setPackageId(pkg.id)}
                className={clsx(
                  'rounded-xl border p-3 text-left transition-colors',
                  packageId === pkg.id ? 'border-purple bg-purple-light/40' : 'border-border hover:border-purple',
                )}
              >
                <p className="text-sm font-bold text-ink">{formatPackageType(pkg.type)}</p>
                <p className="text-xs text-ink-soft">
                  {formatPrice(pkg.basePrice)} + {formatPrice(pkg.pricePerChild)}/detetu
                </p>
              </button>
            ))}
          </div>
        </div>

        {estimatedPrice !== null && (
          <div className="rounded-xl bg-green-light px-4 py-3 text-sm font-semibold text-green-dark">
            Okvirna cena: {formatPrice(estimatedPrice)}
            <span className="ml-1 font-normal text-green-dark/80">(konacan iznos odredjuje server)</span>
          </div>
        )}

        <Button type="submit" disabled={!canSubmit} isLoading={createReservation.isPending}>
          Potvrdi rezervaciju
        </Button>
      </form>
    </Card>
  )
}
