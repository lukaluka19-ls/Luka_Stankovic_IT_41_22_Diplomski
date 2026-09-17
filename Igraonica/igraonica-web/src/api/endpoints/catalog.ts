import { apiClient } from '../client'
import type { components } from '../generated/schema'

type Schemas = components['schemas']

export const packagesApi = {
  getActive: () => apiClient.get<Schemas['PackageDto'][]>('/packages').then((r) => r.data),
}

export const timeSlotsApi = {
  getActive: () => apiClient.get<Schemas['TimeSlotDto'][]>('/timeslots').then((r) => r.data),
}

export const availabilityApi = {
  get: (from: string, to: string) =>
    apiClient
      .get<Schemas['AvailabilityDayDto'][]>('/availability', { params: { from, to } })
      .then((r) => r.data),
}
