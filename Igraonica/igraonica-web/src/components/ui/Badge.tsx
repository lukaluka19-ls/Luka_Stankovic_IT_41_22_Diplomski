import type { ReactNode } from 'react'
import clsx from 'clsx'

interface BadgeProps {
  children: ReactNode
  tone?: 'success' | 'warning' | 'danger' | 'neutral'
}

const TONE_CLASSES: Record<string, string> = {
  success: 'bg-green-light text-green-dark',
  warning: 'bg-cyan-light text-cyan-dark',
  danger: 'bg-danger/10 text-danger',
  neutral: 'bg-purple-light/60 text-ink-soft',
}

export function Badge({ children, tone = 'neutral' }: BadgeProps) {
  return (
    <span className={clsx('inline-flex items-center rounded-full px-3 py-1 text-xs font-semibold', TONE_CLASSES[tone])}>
      {children}
    </span>
  )
}

export function statusTone(status: string): 'success' | 'warning' | 'danger' | 'neutral' {
  switch (status) {
    case 'Confirmed':
    case 'Completed':
      return 'success'
    case 'PendingConfirmation':
      return 'warning'
    case 'CancelledByClient':
    case 'CancelledByAdmin':
    case 'AutoExpired':
      return 'danger'
    default:
      return 'neutral'
  }
}
