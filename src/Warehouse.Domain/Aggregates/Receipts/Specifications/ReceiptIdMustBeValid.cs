using Warehouse.Core.Results;
using Warehouse.Core.Specifications;

namespace Warehouse.Domain.Aggregates.Receipts.Specifications;

public class ReceiptIdMustBeValid(Guid receiptId) : ISpecification
{
    public Result IsSatisfied()
    {
        return receiptId == Guid.Empty 
            ? Result.Failure<string>(ReceiptErrors.EmptyReceiptId) 
            : Result.Success();
    }
}