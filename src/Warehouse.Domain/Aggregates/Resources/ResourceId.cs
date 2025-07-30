using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Resources;

public class ResourceId : TypedId
{
    protected ResourceId() { }

    public ResourceId(Guid value) : base(value)
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}