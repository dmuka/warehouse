namespace Warehouse.Presentation.DTOs;

public class BalanceFilterDto
{
    public List<Guid> ResourceNames { get; set; } = [];
    public List<Guid> UnitNames { get; set; } = [];
}