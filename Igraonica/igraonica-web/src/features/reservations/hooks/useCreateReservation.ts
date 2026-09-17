import { useMutation, useQueryClient } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { reservationsApi } from '../../../api/endpoints/reservations'
import { formatDateTime } from '../../../lib/formatters'
import type { ApiError } from '../../../api/client'
import type { components } from '../../../api/generated/schema'

type CreateRequest = components['schemas']['CreateReservationRequest']
type ReservationDto = components['schemas']['ReservationDto']

export function useCreateReservation() {
  const queryClient = useQueryClient()

  return useMutation<ReservationDto, ApiError, CreateRequest>({
    mutationFn: (payload) => reservationsApi.create(payload),

    onSuccess: (reservation) => {
      // Kalendar dostupnosti i lista rezervacija vise nisu tacni,
      // pa se oznacavaju kao zastareli i ponovo preuzimaju.
      queryClient.invalidateQueries({ queryKey: ['availability'] })
      queryClient.invalidateQueries({ queryKey: ['my-reservations'] })

      toast.success(
        `Rezervacija ${reservation.reservationNumber} je kreirana. ` +
          `Potvrdite je dolaskom u igraonicu do ${formatDateTime(reservation.confirmationDeadline)}.`,
      )
    },

    onError: (error) => {
      if (error.status === 409) {
        toast.error(error.detail ?? 'Termin je upravo zauzet. Izaberite drugi datum.')
        queryClient.invalidateQueries({ queryKey: ['availability'] })
        return
      }
      toast.error(error.detail ?? 'Doslo je do greske pri kreiranju rezervacije.')
    },
  })
}
