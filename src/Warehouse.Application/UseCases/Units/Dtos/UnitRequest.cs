namespace Warehouse.Application.UseCases.Units.Dtos;

public sealed record UnitRequest(Guid Id, string UnitName, bool IsActive);