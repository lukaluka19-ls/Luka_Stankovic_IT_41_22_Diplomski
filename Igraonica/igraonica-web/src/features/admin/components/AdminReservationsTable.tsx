import { useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { adminReservationsApi } from '../../../api/endpoints/reservations'
import { formatDate, formatPrice, formatStatus } from '../../../lib/formatters'
import { Badge, statusTone } from '../../../components/ui/Badge'
import { Button } from '../../../components/ui/Button'

const STATUS_OPTIONS = [
  { value: '', label: 'Svi statusi' },
  { value: 'PendingConfirmation', label: 'Ceka potvrdu' },
  { value: 'Confirmed', label: 'Potvrdjena' },
  { value: 'Completed', label: 'Odrzana' },
  { value: 'CancelledByClient', label: 'Otkazana (klijent)' },
  { value: 'CancelledByAdmin', label: 'Otkazana (igraonica)' },
  { value: 'AutoExpired', label: 'Istekla' },
]

export function AdminReservationsTable() {
  const [status, setStatus] = useState('')
  const [page, setPage] = useState(1)
  const queryClient = useQueryClient()

  const { data, isLoading } = useQuery({
    queryKey: ['admin-reservations', status, page],
    queryFn: () => adminReservationsApi.getAll({ status: status || undefined, page, pageSize: 10 }),
  })

  const confirmMutation = useMutation({
    mutationFn: (id: string) => adminReservationsApi.confirm(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-reservations'] })
      toast.success('Dolazak je potvrdjen.')
    },
    onError: () => toast.error('Potvrda nije uspela.'),
  })

  const cancelMutation = useMutation({
    mutationFn: (id: string) => adminReservationsApi.cancel(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-reservations'] })
      toast.success('Rezervacija je otkazana.')
    },
    onError: () => toast.error('Otkazivanje nije uspelo.'),
  })

  return (
    <div className="rounded-2xl border border-border bg-white p-5 shadow-sm">
      <div className="mb-4 flex flex-wrap items-center justify-between gap-3">
        <h3 className="text-lg font-bold text-ink">Sve rezervacije</h3>
        <select
          value={status}
          onChange={(e) => {
            setStatus(e.target.value)
            setPage(1)
          }}
          className="rounded-lg border border-border px-3 py-2 text-sm"
        >
          {STATUS_OPTIONS.map((opt) => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full min-w-[720px] text-left text-sm">
          <thead>
            <tr className="border-b border-border text-ink-soft">
              <th className="py-2 pr-3 font-semibold">Broj</th>
              <th className="py-2 pr-3 font-semibold">Datum</th>
              <th className="py-2 pr-3 font-semibold">Dete</th>
              <th className="py-2 pr-3 font-semibold">Cena</th>
              <th className="py-2 pr-3 font-semibold">Status</th>
              <th className="py-2 pr-3 font-semibold">Akcije</th>
            </tr>
          </thead>
          <tbody>
            {isLoading && (
              <tr>
                <td colSpan={6} className="py-6 text-center text-ink-soft">
                  Ucitavanje...
                </td>
              </tr>
            )}
            {data?.items.map((r) => (
              <tr key={r.id} className="border-b border-border/60">
                <td className="py-2.5 pr-3 font-mono text-xs">{r.reservationNumber}</td>
                <td className="py-2.5 pr-3">{formatDate(r.eventDate)}</td>
                <td className="py-2.5 pr-3">{r.childName}</td>
                <td className="py-2.5 pr-3">{formatPrice(r.totalPrice)}</td>
                <td className="py-2.5 pr-3">
                  <Badge tone={statusTone(r.status)}>{formatStatus(r.status)}</Badge>
                </td>
                <td className="py-2.5 pr-3">
                  <div className="flex gap-2">
                    {r.status === 'PendingConfirmation' && (
                      <>
                        <Button
                          variant="secondary"
                          className="px-3 py-1.5 text-xs"
                          onClick={() => confirmMutation.mutate(r.id)}
                          isLoading={confirmMutation.isPending}
                        >
                          Potvrdi dolazak
                        </Button>
                        <Button
                          variant="danger"
                          className="px-3 py-1.5 text-xs"
                          onClick={() => cancelMutation.mutate(r.id)}
                          isLoading={cancelMutation.isPending}
                        >
                          Otkazi
                        </Button>
                      </>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {data && Number(data.totalPages) > 1 && (
        <div className="mt-4 flex items-center justify-center gap-3 text-sm">
          <button
            disabled={page <= 1}
            onClick={() => setPage((p) => p - 1)}
            className="rounded-full px-3 py-1 disabled:opacity-30"
          >
            &larr;
          </button>
          <span>
            Strana {data.page} od {data.totalPages}
          </span>
          <button
            disabled={page >= Number(data.totalPages)}
            onClick={() => setPage((p) => p + 1)}
            className="rounded-full px-3 py-1 disabled:opacity-30"
          >
            &rarr;
          </button>
        </div>
      )}
    </div>
  )
}
