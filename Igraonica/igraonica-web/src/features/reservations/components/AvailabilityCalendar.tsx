import { useMemo, useState } from 'react'
import clsx from 'clsx'
import { useAvailability } from '../hooks/useAvailability'

interface AvailabilityCalendarProps {
  selectedDate: string | null
  onSelectDate: (date: string) => void
}

function toIso(date: Date): string {
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`
}

const MONTH_NAMES = [
  'Januar', 'Februar', 'Mart', 'April', 'Maj', 'Jun',
  'Jul', 'Avgust', 'Septembar', 'Oktobar', 'Novembar', 'Decembar',
]
const WEEKDAY_LABELS = ['Pon', 'Uto', 'Sre', 'Cet', 'Pet', 'Sub', 'Ned']

export function AvailabilityCalendar({ selectedDate, onSelectDate }: AvailabilityCalendarProps) {
  const today = useMemo(() => new Date(), [])
  const [viewDate, setViewDate] = useState(() => new Date(today.getFullYear(), today.getMonth(), 1))

  const monthStart = new Date(viewDate.getFullYear(), viewDate.getMonth(), 1)
  const monthEnd = new Date(viewDate.getFullYear(), viewDate.getMonth() + 1, 0)
  const { data: days, isLoading } = useAvailability(toIso(monthStart), toIso(monthEnd))

  const availabilityByDate = useMemo(() => {
    const map = new Map<string, { capacity: number; booked: number; remaining: number; isWeekend: boolean }>()
    days?.forEach((d) =>
      map.set(d.date, {
        capacity: Number(d.capacity),
        booked: Number(d.booked),
        remaining: Number(d.remaining),
        isWeekend: d.isWeekend,
      }),
    )
    return map
  }, [days])

  const leadingBlanks = (monthStart.getDay() + 6) % 7 // ponedeljak = 0
  const cells: (Date | null)[] = [
    ...Array.from({ length: leadingBlanks }, () => null),
    ...Array.from({ length: monthEnd.getDate() }, (_, i) => new Date(viewDate.getFullYear(), viewDate.getMonth(), i + 1)),
  ]

  return (
    <div className="rounded-2xl border border-border bg-white p-5 shadow-sm">
      <div className="mb-4 flex items-center justify-between">
        <button
          type="button"
          onClick={() => setViewDate(new Date(viewDate.getFullYear(), viewDate.getMonth() - 1, 1))}
          className="rounded-full px-3 py-1 text-ink-soft hover:bg-purple-light/60"
          aria-label="Prethodni mesec"
        >
          &larr;
        </button>
        <h3 className="text-base font-bold text-ink">
          {MONTH_NAMES[viewDate.getMonth()]} {viewDate.getFullYear()}.
        </h3>
        <button
          type="button"
          onClick={() => setViewDate(new Date(viewDate.getFullYear(), viewDate.getMonth() + 1, 1))}
          className="rounded-full px-3 py-1 text-ink-soft hover:bg-purple-light/60"
          aria-label="Sledeci mesec"
        >
          &rarr;
        </button>
      </div>

      <div className="mb-2 grid grid-cols-7 gap-1 text-center text-xs font-semibold text-ink-soft">
        {WEEKDAY_LABELS.map((label) => (
          <span key={label}>{label}</span>
        ))}
      </div>

      <div className="grid grid-cols-7 gap-1">
        {cells.map((date, idx) => {
          if (!date) return <span key={`blank-${idx}`} />

          const iso = toIso(date)
          const info = availabilityByDate.get(iso)
          const isPast = date < new Date(today.getFullYear(), today.getMonth(), today.getDate())
          const isFull = info ? info.remaining <= 0 : false
          const isSelected = selectedDate === iso
          const disabled = isPast || isFull || isLoading

          return (
            <button
              key={iso}
              type="button"
              disabled={disabled}
              onClick={() => onSelectDate(iso)}
              className={clsx(
                'flex flex-col items-center rounded-xl border py-2 text-xs transition-colors',
                disabled && 'cursor-not-allowed border-transparent text-ink-soft/50',
                !disabled && !isSelected && 'border-border hover:border-purple hover:bg-purple-light/40',
                isSelected && 'border-purple bg-purple text-white',
                !disabled && info && info.remaining > 0 && !isSelected && info.isWeekend && 'bg-cyan/10',
              )}
            >
              <span className="font-bold">{date.getDate()}</span>
              {!isPast && info && (
                <span className={clsx('mt-0.5', isSelected ? 'text-white/90' : isFull ? 'text-danger' : 'text-green-dark')}>
                  {isFull ? 'popunjeno' : `${info.remaining} mesta`}
                </span>
              )}
            </button>
          )
        })}
      </div>

      <div className="mt-4 flex flex-wrap gap-4 text-xs text-ink-soft">
        <span className="flex items-center gap-1.5">
          <span className="h-2.5 w-2.5 rounded-full bg-green" /> Slobodno
        </span>
        <span className="flex items-center gap-1.5">
          <span className="h-2.5 w-2.5 rounded-full bg-cyan" /> Vikend (3 termina)
        </span>
        <span className="flex items-center gap-1.5">
          <span className="h-2.5 w-2.5 rounded-full bg-danger" /> Popunjeno
        </span>
      </div>
    </div>
  )
}
