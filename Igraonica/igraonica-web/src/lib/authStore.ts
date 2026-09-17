import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { components } from '../api/generated/schema'

type UserDto = components['schemas']['UserDto']

interface AuthState {
  accessToken: string | null
  refreshToken: string | null
  user: UserDto | null
  setSession: (accessToken: string, refreshToken: string, user: UserDto) => void
  logout: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      accessToken: null,
      refreshToken: null,
      user: null,
      setSession: (accessToken, refreshToken, user) => set({ accessToken, refreshToken, user }),
      logout: () => set({ accessToken: null, refreshToken: null, user: null }),
    }),
    { name: 'igraonica-auth' },
  ),
)
