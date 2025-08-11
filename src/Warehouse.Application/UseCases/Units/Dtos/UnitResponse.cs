namespace Warehouse.Application.UseCases.Units.Dtos;

public sealed record UnitResponse(Guid Id, string UnitName, bool IsActive);