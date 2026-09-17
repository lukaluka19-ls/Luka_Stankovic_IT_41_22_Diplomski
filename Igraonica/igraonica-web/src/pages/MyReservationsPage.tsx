import { useState } from 'react'
import { useMyReservations, useCancelReservation } from '../features/reservations/hooks/useMyReservations'
import { formatDate, formatPrice, formatStatus } from '../lib/formatters'
import { Badge, statusTone } from '../components/ui/Badge'
import { Button } from '../components/ui/Button'
import { Card } from '../components/ui/Card'
import { DeadlineCountdown } from '../features/reservations/components/DeadlineCountdown'

export function MyReservationsPage() {
  const { data, isLoading } = useMyReservations()
  const cancelReservation = useCancelReservation()
  const [cancelTarget, setCancelTarget] = useState<string | null>(null)
  const [reason, setReason] = useState('')

  return (
    <div className="mx-auto max-w-3xl px-4 py-10 sm:px-6">
      <h1 className="mb-6 text-3xl font-black text-ink">Moje rezervacije</h1>

      {isLoading && <p className="text-ink-soft">Ucitavanje...</p>}
      {!isLoading && data?.items.length === 0 && (
        <p className="text-ink-soft">Jos uvek nemate nijednu rezervaciju.</p>
      )}

      <div className="flex flex-col gap-4">
        {data?.items.map((r) => (
          <Card key={r.id} className="flex flex-col gap-2">
            <div className="flex flex-wrap items-center justify-between gap-2">
              <span className="font-mono text-xs text-ink-soft">{r.reservationNumber}</span>
              <Badge tone={statusTone(r.status)}>{formatStatus(r.status)}</Badge>
            </div>
            <h3 className="text-lg font-bold text-ink">Proslava za {r.childName}</h3>
            <p className="text-sm text-ink-soft">
              {formatDate(r.eventDate)} &middot; {r.timeSlotName} &middot; {r.packageName} &middot;{' '}
              {formatPrice(r.totalPrice)}
            </p>

            {r.status === 'PendingConfirmation' && (
              <div className="mt-1 flex flex-wrap items-center gap-3">
                <DeadlineCountdown daysUntilDeadline={r.daysUntilDeadline} />
                {cancelTarget === r.id ? (
                  <form
                    className="flex w-full flex-col gap-2 sm:flex-row"
                    onSubmit={(e) => {
                      e.preventDefault()
                      cancelReservation.mutate(
                        { id: r.id, reason },
                        { onSuccess: () => setCancelTarget(null) },
                      )
                    }}
                  >
                    <input
                      value={reason}
                      onChange={(e) => setReason(e.target.value)}
                      placeholder="Razlog otkazivanja (najmanje 10 znakova)"
                      className="flex-1 rounded-lg border border-border px-3 py-2 text-sm"
                      minLength={10}
                      required
                    />
                    <Button type="submit" variant="danger" isLoading={cancelReservation.isPending}>
                      Potvrdi otkazivanje
                    </Button>
                  </form>
                ) : (
                  <Button variant="ghost" className="ml-auto" onClick={() => setCancelTarget(r.id)}>
                    Otkazi rezervaciju
                  </Button>
                )}
              </div>
            )}
          </Card>
        ))}
      </div>
    </div>
  )
}
