using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Balances;

public class BalanceId : TypedId
{
    protected BalanceId() { }
    public BalanceId(Guid value) : base(value) { }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}