import { apiClient } from '../client'
import type { components } from '../generated/schema'

type Schemas = components['schemas']

export const adminPackagesApi = {
  getAll: () => apiClient.get<Schemas['PackageDto'][]>('/admin/packages').then((r) => r.data),
  update: (id: string, payload: Schemas['UpdatePackageRequest']) =>
    apiClient.put<Schemas['PackageDto']>(`/admin/packages/${id}`, payload).then((r) => r.data),
}

export const adminTimeSlotsApi = {
  getAll: () => apiClient.get<Schemas['TimeSlotDto'][]>('/admin/timeslots').then((r) => r.data),
  create: (payload: Schemas['CreateTimeSlotRequest']) =>
    apiClient.post<Schemas['TimeSlotDto']>('/admin/timeslots', payload).then((r) => r.data),
}

export const adminStatisticsApi = {
  get: () => apiClient.get<Schemas['StatisticsDto']>('/admin/statistics').then((r) => r.data),
}

export const adminAuditLogsApi = {
  getAll: (page = 1, pageSize = 20) =>
    apiClient
      .get<Schemas['PagedResultDtoOfAuditLogDto']>('/admin/audit-logs', { params: { page, pageSize } })
      .then((r) => r.data),
}
