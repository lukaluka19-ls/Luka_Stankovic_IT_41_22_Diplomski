import { useMutation } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { authApi } from '../../../api/endpoints/auth'
import { useAuthStore } from '../../../lib/authStore'
import type { ApiError } from '../../../api/client'
import type { components } from '../../../api/generated/schema'

type RegisterRequest = components['schemas']['RegisterRequest']
type LoginRequest = components['schemas']['LoginRequest']

export function useLogin() {
  const setSession = useAuthStore((s) => s.setSession)

  return useMutation<components['schemas']['TokenResponseDto'], ApiError, LoginRequest>({
    mutationFn: authApi.login,
    onSuccess: (data) => {
      setSession(data.accessToken, data.refreshToken, data.user)
      toast.success(`Dobrodosli, ${data.user.firstName}!`)
    },
    onError: (error) => {
      toast.error(error.detail ?? 'Pogresna adresa elektronske poste ili lozinka.')
    },
  })
}

export function useRegister() {
  return useMutation<components['schemas']['UserDto'], ApiError, RegisterRequest>({
    mutationFn: authApi.register,
    onSuccess: () => {
      toast.success('Nalog je kreiran. Proverite prijemno sanduce za poruku sa linkom za potvrdu.')
    },
    onError: (error) => {
      toast.error(error.detail ?? 'Registracija nije uspela.')
    },
  })
}
