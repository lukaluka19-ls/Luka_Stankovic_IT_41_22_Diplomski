import { apiClient } from '../client'
import type { components } from '../generated/schema'

type Schemas = components['schemas']

export const authApi = {
  register: (payload: Schemas['RegisterRequest']) =>
    apiClient.post<Schemas['UserDto']>('/auth/register', payload).then((r) => r.data),

  login: (payload: Schemas['LoginRequest']) =>
    apiClient.post<Schemas['TokenResponseDto']>('/auth/login', payload).then((r) => r.data),

  verifyEmail: (payload: Schemas['VerifyEmailRequest']) =>
    apiClient.post('/auth/verify-email', payload).then((r) => r.data),

  resendVerification: (payload: Schemas['ResendVerificationRequest']) =>
    apiClient.post('/auth/resend-verification', payload).then((r) => r.data),

  forgotPassword: (payload: Schemas['ForgotPasswordRequest']) =>
    apiClient.post('/auth/forgot-password', payload).then((r) => r.data),

  resetPassword: (payload: Schemas['ResetPasswordRequest']) =>
    apiClient.post('/auth/reset-password', payload).then((r) => r.data),
}
