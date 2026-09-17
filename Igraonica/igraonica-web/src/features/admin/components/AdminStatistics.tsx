import { useQuery } from '@tanstack/react-query'
import clsx from 'clsx'
import { adminStatisticsApi } from '../../../api/endpoints/admin'
import { formatPrice } from '../../../lib/formatters'

export function AdminStatistics() {
  const { data } = useQuery({ queryKey: ['admin-statistics'], queryFn: adminStatisticsApi.get })

  if (!data) return null

  const tiles = [
    { label: 'Ukupno rezervacija', value: data.totalReservations, accent: 'text-purple-dark' },
    { label: 'Ceka potvrdu', value: data.pendingConfirmation, accent: 'text-cyan-dark' },
    { label: 'Potvrdjeno', value: data.confirmed, accent: 'text-green-dark' },
    { label: 'Odrzano', value: data.completed, accent: 'text-green-dark' },
    { label: 'Otkazano', value: Number(data.cancelledByClient) + Number(data.cancelledByAdmin), accent: 'text-danger' },
    { label: 'Isteklo', value: data.autoExpired, accent: 'text-ink-soft' },
  ]

  return (
    <div className="mb-6 grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-6">
      {tiles.map((tile) => (
        <div
          key={tile.label}
          className="rounded-2xl border border-border bg-white p-4 text-center shadow-sm shadow-navy/5 transition-shadow hover:shadow-md hover:shadow-navy/10"
        >
          <p className={clsx('text-2xl font-black', tile.accent)}>{tile.value}</p>
          <p className="text-xs text-ink-soft">{tile.label}</p>
        </div>
      ))}
      <div className="col-span-2 rounded-2xl border border-border bg-gradient-to-br from-green-light to-cyan-light p-4 text-center shadow-sm shadow-navy/5 sm:col-span-3 lg:col-span-6">
        <p className="text-2xl font-black text-green-dark">{formatPrice(data.totalRevenue)}</p>
        <p className="text-xs text-ink-soft">Ukupan prihod od potvrdjenih i odrzanih proslava</p>
      </div>
    </div>
  )
}
