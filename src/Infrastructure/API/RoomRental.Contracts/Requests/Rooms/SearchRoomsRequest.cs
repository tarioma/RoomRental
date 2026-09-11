namespace RoomRental.Contracts.Requests.Rooms;

/// <summary>
/// Параметры выборки списка залов. Занятость не учитывается.
/// </summary>
public record SearchRoomsRequest : PagedRequest;
