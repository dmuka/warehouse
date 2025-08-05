using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Application.UseCases.Receipts.Specifications;

public class ReceiptNumberMustBeUnique(string receiptNumber, IReceiptRepository repository) : IAsyncSpecification
{
    public async Task<Result> IsSatisfiedAsync(CancellationToken cancellationToken)
    {
        if (!await repository.IsNumberUniqueAsync(receiptNumber))
            return Result.Failure<ReceiptId>(ReceiptErrors.ReceiptAlreadyExist(receiptNumber));
        
        return Result.Success();
    }
}