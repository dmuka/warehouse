namespace Warehouse.Application.UseCases.Balances.Dtos;

public class AvailableBalanceRequest
{
    public Guid ResourceId { get; set; }
    public Guid UnitId { get; set; }
}