import { AdminStatistics } from '../features/admin/components/AdminStatistics'
import { AdminReservationsTable } from '../features/admin/components/AdminReservationsTable'

export function AdminDashboardPage() {
  return (
    <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6">
      <h1 className="mb-6 text-3xl font-black text-ink">Administratorski pregled</h1>
      <AdminStatistics />
      <AdminReservationsTable />
    </div>
  )
}
