using Warehouse.Core;

namespace Warehouse.Application.UseCases.Balances.Dtos;

public sealed record BalanceResponse(
    Guid Id,
    Guid ResourceId,
    string ResourceName,
    Guid UnitId,
    string UnitName,
    decimal Quantity);