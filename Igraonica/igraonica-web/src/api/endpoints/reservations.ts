import { apiClient } from '../client'
import type { components } from '../generated/schema'

type Schemas = components['schemas']

export const reservationsApi = {
  create: (payload: Schemas['CreateReservationRequest']) =>
    apiClient.post<Schemas['ReservationDto']>('/reservations', payload).then((r) => r.data),

  getMy: (page = 1, pageSize = 20) =>
    apiClient
      .get<Schemas['PagedResultDtoOfReservationDto']>('/reservations/my', { params: { page, pageSize } })
      .then((r) => r.data),

  getById: (id: string) =>
    apiClient.get<Schemas['ReservationDto']>(`/reservations/${id}`).then((r) => r.data),

  cancel: (id: string, reason: string) =>
    apiClient.post(`/reservations/${id}/cancel`, { reason }).then((r) => r.data),
}

export const adminReservationsApi = {
  getAll: (params: { status?: string; fromDate?: string; toDate?: string; page?: number; pageSize?: number }) =>
    apiClient
      .get<Schemas['PagedResultDtoOfReservationDto']>('/admin/reservations', { params })
      .then((r) => r.data),

  confirm: (id: string) =>
    apiClient.post<Schemas['ReservationDto']>(`/admin/reservations/${id}/confirm`).then((r) => r.data),

  cancel: (id: string, reason?: string) =>
    apiClient.delete(`/admin/reservations/${id}`, { params: reason ? { reason } : undefined }).then((r) => r.data),
}
