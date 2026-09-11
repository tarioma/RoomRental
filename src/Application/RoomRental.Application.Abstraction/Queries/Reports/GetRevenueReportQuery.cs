using MediatR;
using RoomRental.Application.Abstraction.Dto;

namespace RoomRental.Application.Abstraction.Queries.Reports;

/// <summary>
/// Выручка за период с разрезами по залам и услугам.
/// </summary>
/// <param name="From">Начало периода, включительно.</param>
/// <param name="To">Окончание периода, включительно.</param>
public record GetRevenueReportQuery(DateOnly From, DateOnly To) : IRequest<RevenueReportDto>;
