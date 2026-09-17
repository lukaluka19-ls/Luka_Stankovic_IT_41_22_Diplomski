import type { ButtonHTMLAttributes, ReactNode } from 'react'
import clsx from 'clsx'

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'ghost' | 'ghost-dark' | 'danger'
  children: ReactNode
  isLoading?: boolean
}

const VARIANT_CLASSES: Record<string, string> = {
  primary: 'bg-purple text-white shadow-md shadow-purple/30 hover:bg-purple-dark hover:shadow-lg hover:shadow-purple/40 hover:-translate-y-0.5',
  secondary: 'bg-green text-white shadow-md shadow-green/30 hover:bg-green-dark hover:shadow-lg hover:shadow-green/40 hover:-translate-y-0.5',
  ghost: 'bg-transparent text-ink border border-border hover:bg-purple-light/60 hover:border-purple/40',
  'ghost-dark': 'bg-transparent text-white border border-white/25 hover:bg-white/10 hover:border-white/40',
  danger: 'bg-danger text-white shadow-md shadow-danger/30 hover:opacity-90 hover:-translate-y-0.5',
}

export function Button({ variant = 'primary', className, children, isLoading, disabled, ...props }: ButtonProps) {
  return (
    <button
      className={clsx(
        'inline-flex items-center justify-center gap-2 rounded-full px-5 py-2.5 font-semibold text-sm transition-all duration-200 active:translate-y-0 active:scale-[0.98] disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:translate-y-0 disabled:hover:shadow-none',
        VARIANT_CLASSES[variant],
        className,
      )}
      disabled={disabled || isLoading}
      {...props}
    >
      {isLoading && (
        <span className="h-4 w-4 animate-spin rounded-full border-2 border-white/40 border-t-white" />
      )}
      {children}
    </button>
  )
}
