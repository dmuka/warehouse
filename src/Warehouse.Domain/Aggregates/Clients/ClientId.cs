using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Clients;

public class ClientId : TypedId
{
    protected ClientId() { }
    public ClientId(Guid value) : base(value) { }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}