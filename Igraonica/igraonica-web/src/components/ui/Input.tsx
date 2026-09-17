import { type InputHTMLAttributes, forwardRef } from 'react'
import clsx from 'clsx'

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string
  error?: string
}

export const Input = forwardRef<HTMLInputElement, InputProps>(function Input(
  { label, error, className, id, ...props },
  ref,
) {
  return (
    <label className="flex flex-col gap-1 text-left" htmlFor={id}>
      {label && <span className="text-sm font-medium text-ink">{label}</span>}
      <input
        ref={ref}
        id={id}
        className={clsx(
          'rounded-xl border border-border bg-white px-3.5 py-2.5 text-sm outline-none transition-colors focus:border-purple',
          error && 'border-danger',
          className,
        )}
        {...props}
      />
      {error && <span className="text-xs text-danger">{error}</span>}
    </label>
  )
})
