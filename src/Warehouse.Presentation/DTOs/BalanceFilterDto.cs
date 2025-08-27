namespace Warehouse.Presentation.DTOs;

public class BalanceFilterDto
{
    public List<Guid> ResourceIds { get; set; } = [];
    public List<Guid> UnitIds { get; set; } = [];
}