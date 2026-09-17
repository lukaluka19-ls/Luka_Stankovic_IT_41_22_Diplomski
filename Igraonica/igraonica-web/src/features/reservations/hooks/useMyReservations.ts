import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { reservationsApi } from '../../../api/endpoints/reservations'
import type { ApiError } from '../../../api/client'

export function useMyReservations(page = 1) {
  return useQuery({
    queryKey: ['my-reservations', page],
    queryFn: () => reservationsApi.getMy(page),
  })
}

export function useCancelReservation() {
  const queryClient = useQueryClient()

  return useMutation<void, ApiError, { id: string; reason: string }>({
    mutationFn: ({ id, reason }) => reservationsApi.cancel(id, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['my-reservations'] })
      queryClient.invalidateQueries({ queryKey: ['availability'] })
      toast.success('Rezervacija je otkazana.')
    },
    onError: (error) => {
      toast.error(error.detail ?? 'Otkazivanje nije uspelo.')
    },
  })
}
