using Warehouse.Core.Results;
using Warehouse.Core.Specifications;

namespace Warehouse.Domain.Aggregates.Receipts.Specifications;

public class ReceiptNumberMustBeValid(string receiptNumber) : ISpecification
{
    public Result IsSatisfied()
    {
        return string.IsNullOrWhiteSpace(receiptNumber) 
            ? Result.Failure(ReceiptErrors.EmptyReceiptNumber) 
            : Result.Success();
    }
}