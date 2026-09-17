export function formatDate(value: string | Date): string {
  const date = typeof value === 'string' ? new Date(value) : value
  return date.toLocaleDateString('sr-Latn-RS', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

export function formatDateTime(value: string | Date): string {
  const date = typeof value === 'string' ? new Date(value) : value
  return date.toLocaleString('sr-Latn-RS', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

export function formatPrice(value: number | string): string {
  // ASP.NET Core 10 opisuje decimal/int polja u OpenAPI dokumentu kao
  // "number | string" radi ocuvanja preciznosti pri razmeni podataka,
  // iako se u praksi uvek serijalizuju kao brojevi.
  return `${Number(value).toLocaleString('sr-Latn-RS')} RSD`
}

const STATUS_LABELS: Record<string, string> = {
  PendingConfirmation: 'Ceka potvrdu',
  Confirmed: 'Potvrdjena',
  CancelledByClient: 'Otkazana (klijent)',
  CancelledByAdmin: 'Otkazana (igraonica)',
  AutoExpired: 'Istekla',
  Completed: 'Odrzana',
}

export function formatStatus(status: string): string {
  return STATUS_LABELS[status] ?? status
}

const PACKAGE_TYPE_LABELS: Record<string, string> = {
  Basic: 'Osnovni',
  Standard: 'Standardni',
  Premium: 'Premium',
}

export function formatPackageType(type: string): string {
  return PACKAGE_TYPE_LABELS[type] ?? type
}
