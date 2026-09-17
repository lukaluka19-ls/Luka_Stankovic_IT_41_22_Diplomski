interface DeadlineCountdownProps {
  daysUntilDeadline: number | string | null | undefined
}

export function DeadlineCountdown({ daysUntilDeadline: raw }: DeadlineCountdownProps) {
  if (raw === null || raw === undefined) return null
  const daysUntilDeadline = Number(raw)

  const urgent = daysUntilDeadline <= 2

  return (
    <span
      className={
        'inline-flex items-center gap-1.5 rounded-full px-3 py-1 text-xs font-bold ' +
        (urgent ? 'bg-danger/10 text-danger' : 'bg-cyan-light text-cyan-dark')
      }
    >
      {daysUntilDeadline === 0
        ? 'Rok istice danas'
        : `Jos ${daysUntilDeadline} ${daysUntilDeadline === 1 ? 'dan' : 'dana'} za potvrdu`}
    </span>
  )
}
