import { useQuery } from '@tanstack/react-query'
import { availabilityApi } from '../../../api/endpoints/catalog'

export function useAvailability(from: string, to: string) {
  return useQuery({
    queryKey: ['availability', from, to],
    queryFn: () => availabilityApi.get(from, to),
  })
}
