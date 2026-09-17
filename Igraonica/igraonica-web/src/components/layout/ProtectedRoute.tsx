import type { ReactNode } from 'react'
import { Navigate } from 'react-router-dom'
import { useAuthStore } from '../../lib/authStore'

interface ProtectedRouteProps {
  children: ReactNode
  requireRole?: 'Client' | 'Administrator'
}

export function ProtectedRoute({ children, requireRole }: ProtectedRouteProps) {
  const user = useAuthStore((s) => s.user)

  if (!user) return <Navigate to="/prijava" replace />
  if (requireRole && user.role !== requireRole) return <Navigate to="/" replace />

  return <>{children}</>
}
