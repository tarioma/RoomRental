using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Domain.Entities;
using RoomRental.Domain.Enums;

namespace RoomRental.Application.Authorization;

/// <summary>
/// Правило доступа к чужим броням. Вынесено в одно место,
/// чтобы просмотр и отмена не разъехались между собой.
/// </summary>
internal static class BookingAccess
{
    /// <summary>
    /// Пропускает владельца брони и администратора, остальным отказывает.
    /// </summary>
    /// <exception cref="AccessDeniedException">Бронь принадлежит другому пользователю.</exception>
    public static void EnsureOwnerOrAdmin(Booking booking, Guid requesterId, UserRole requesterRole)
    {
        if (booking.CustomerId == requesterId || requesterRole == UserRole.Admin)
        {
            return;
        }

        throw new AccessDeniedException("Бронь принадлежит другому пользователю.");
    }
}
