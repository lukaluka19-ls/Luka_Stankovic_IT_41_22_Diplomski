import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios'
import { useAuthStore } from '../lib/authStore'

export interface ApiError {
  status?: number
  detail?: string
  errorCode?: string
}

interface RetriableRequest extends InternalAxiosRequestConfig {
  _retry?: boolean
}

export const apiClient = axios.create({
  baseURL: '/api',
})

apiClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().accessToken
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

let refreshPromise: Promise<string> | null = null

async function refreshAccessToken(): Promise<string> {
  const refreshToken = useAuthStore.getState().refreshToken
  if (!refreshToken) throw new Error('Nema tokena za osvezavanje.')

  const response = await axios.post('/api/auth/refresh', { refreshToken })
  const { accessToken, refreshToken: newRefreshToken, user } = response.data
  useAuthStore.getState().setSession(accessToken, newRefreshToken, user)
  return accessToken
}

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const original = error.config as RetriableRequest

    if (error.response?.status !== 401 || original._retry || original.url?.includes('/auth/')) {
      return Promise.reject(toApiError(error))
    }
    original._retry = true

    // Bez ove provere bi svaki zahtev koji dobije odgovor 401 pokrenuo
    // zaseban pokusaj obnavljanja tokena. Obnavljanje se pokrece samo
    // jednom, dok ostali zahtevi cekaju njegov ishod.
    refreshPromise ??= refreshAccessToken().finally(() => {
      refreshPromise = null
    })

    try {
      const token = await refreshPromise
      original.headers.Authorization = `Bearer ${token}`
      return apiClient(original)
    } catch {
      useAuthStore.getState().logout()
      window.location.href = '/prijava'
      return Promise.reject(toApiError(error))
    }
  },
)

function toApiError(error: AxiosError): ApiError {
  const data = error.response?.data as { detail?: string; errorCode?: string } | undefined
  return {
    status: error.response?.status,
    detail: data?.detail,
    errorCode: data?.errorCode,
  }
}
