namespace Warehouse.Application.UseCases.Balances.Dtos;

public sealed record BalanceRequest(
    Guid Id,
    string ResourceName,
    string UnitName,
    decimal Quantity);