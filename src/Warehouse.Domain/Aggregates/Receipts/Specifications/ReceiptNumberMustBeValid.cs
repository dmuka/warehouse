using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Domain.Aggregates.Receipts.Specifications;

public class ReceiptNumberMustBeValid(string receiptNumber) : ISpecification
{
    public Result IsSatisfied()
    {
        return string.IsNullOrWhiteSpace(receiptNumber) 
            ? Result.Failure(ClientErrors.EmptyAddress) 
            : Result.Success();
    }
}