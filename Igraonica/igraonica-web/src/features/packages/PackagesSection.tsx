import { useQuery } from '@tanstack/react-query'
import clsx from 'clsx'
import { packagesApi } from '../../api/endpoints/catalog'
import { formatPackageType, formatPrice } from '../../lib/formatters'

const ACCENT_BAR: Record<string, string> = {
  Basic: 'bg-cyan',
  Standard: 'bg-purple',
  Premium: 'bg-green',
}

const ACCENT_BADGE: Record<string, string> = {
  Basic: 'bg-cyan-light text-cyan-dark',
  Standard: 'bg-purple-light text-purple-dark',
  Premium: 'bg-green-light text-green-dark',
}

export function PackagesSection() {
  const { data: packages, isLoading } = useQuery({ queryKey: ['packages'], queryFn: packagesApi.getActive })

  if (isLoading) return <p className="text-center text-ink-soft">Ucitavanje ponude...</p>

  return (
    <div className="grid grid-cols-1 gap-6 sm:grid-cols-3">
      {packages?.map((pkg) => (
        <div
          key={pkg.id}
          className="flex flex-col overflow-hidden rounded-2xl border border-border bg-white text-left shadow-sm shadow-navy/5 transition-all duration-200 hover:-translate-y-1 hover:shadow-lg hover:shadow-navy/10"
        >
          <div className={clsx('h-1.5 w-full', ACCENT_BAR[pkg.type] ?? 'bg-purple')} />
          <div className="flex flex-1 flex-col gap-2 p-6">
            <span
              className={clsx(
                'w-fit rounded-full px-3 py-1 text-xs font-bold',
                ACCENT_BADGE[pkg.type] ?? 'bg-purple-light text-purple-dark',
              )}
            >
              {formatPackageType(pkg.type)} paket
            </span>
            <h3 className="text-lg font-bold text-ink">{pkg.name}</h3>
            <p className="flex-1 text-sm text-ink-soft">{pkg.description}</p>
            <div className="mt-2 border-t border-border pt-3">
              <p className="text-2xl font-black text-ink">
                {formatPrice(pkg.basePrice)}
                <span className="ml-1 text-sm font-normal text-ink-soft">osnovna cena</span>
              </p>
              <p className="text-sm font-semibold text-green-dark">
                + {formatPrice(pkg.pricePerChild)} <span className="font-normal text-ink-soft">po detetu</span>
              </p>
            </div>
          </div>
        </div>
      ))}
    </div>
  )
}
