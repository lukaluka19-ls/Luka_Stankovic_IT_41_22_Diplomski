import { useEffect, useState } from 'react'
import { useSearchParams, Link } from 'react-router-dom'
import { authApi } from '../api/endpoints/auth'
import { Card } from '../components/ui/Card'
import { Button } from '../components/ui/Button'

export function VerifyEmailPage() {
  const [searchParams] = useSearchParams()
  const token = searchParams.get('token')
  const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading')

  useEffect(() => {
    if (!token) {
      setStatus('error')
      return
    }
    authApi
      .verifyEmail({ token })
      .then(() => setStatus('success'))
      .catch(() => setStatus('error'))
  }, [token])

  return (
    <div className="mx-auto max-w-md px-4 py-16 sm:px-6">
      <Card className="text-center">
        {status === 'loading' && <p className="text-ink-soft">Potvrdjivanje adrese...</p>}
        {status === 'success' && (
          <>
            <h2 className="mb-2 text-xl font-bold text-green-dark">Adresa je potvrdjena!</h2>
            <p className="mb-4 text-sm text-ink-soft">Sada mozete da se prijavite i napravite rezervaciju.</p>
            <Link to="/prijava">
              <Button>Prijavi se</Button>
            </Link>
          </>
        )}
        {status === 'error' && (
          <>
            <h2 className="mb-2 text-xl font-bold text-danger">Link nije ispravan</h2>
            <p className="text-sm text-ink-soft">
              Token za potvrdu nije vazeci ili je istekao. Zatrazite novu poruku za potvrdu sa stranice za prijavu.
            </p>
          </>
        )}
      </Card>
    </div>
  )
}
