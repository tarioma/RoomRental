using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Queries.Rooms;
using RoomRental.Application.Abstraction.Repositories;

namespace RoomRental.Application.UseCases.Rooms;

/// <summary>
/// Возвращает список действующих залов постранично.
/// </summary>
/// <param name="rooms">Репозиторий залов.</param>
public class SearchRoomsCase(
    IRoomRepository rooms)
    : IRequestHandler<SearchRoomsQuery, IReadOnlyList<RoomDto>>
{
    /// <inheritdoc />
    public Task<IReadOnlyList<RoomDto>> Handle(SearchRoomsQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        return rooms.SearchAsync(request.Filters, cancellationToken);
    }
}
