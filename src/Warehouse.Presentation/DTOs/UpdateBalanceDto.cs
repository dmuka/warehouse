namespace Warehouse.Presentation.DTOs;

public sealed record UpdateBalanceDto(Guid ResourceId, Guid UnitId, decimal Quantity);