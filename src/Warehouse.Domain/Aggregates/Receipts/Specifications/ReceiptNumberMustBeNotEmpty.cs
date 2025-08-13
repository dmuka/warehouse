using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Application.UseCases.Receipts.Specifications;

public class ReceiptNumberMustBeNotEmpty(string receiptNumber) : ISpecification
{
    public Result IsSatisfied()
    {
        if (string.IsNullOrEmpty(receiptNumber) || string.IsNullOrWhiteSpace(receiptNumber))
            return Result.Failure<ReceiptId>(ReceiptErrors.ReceiptNumberNotValid(receiptNumber));
        
        return Result.Success();
    }
}