namespace Warehouse.Application.UseCases.Balances.Dtos;

public class BalanceResponse(
    Guid Id,
    string ResourceName,
    string UnitName,
    decimal Quantity);