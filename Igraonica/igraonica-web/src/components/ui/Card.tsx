import type { ReactNode } from 'react'
import clsx from 'clsx'

export function Card({ children, className }: { children: ReactNode; className?: string }) {
  return (
    <div
      className={clsx(
        'rounded-2xl border border-border bg-white p-6 shadow-sm shadow-navy/5 transition-shadow duration-200 hover:shadow-md hover:shadow-navy/10',
        className,
      )}
    >
      {children}
    </div>
  )
}
