using MediatR;
using Warehouse.Application.UseCases.Receipts.Specifications;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Application.UseCases.Receipts;

public record CreateReceiptCommand(
    string Number,
    DateTime Date) : IRequest<Result<ReceiptId>>;

public sealed class CreateReceiptCommandHandler(
    IReceiptRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateReceiptCommand, Result<ReceiptId>>
{
    public async Task<Result<ReceiptId>> Handle(
        CreateReceiptCommand request,
        CancellationToken cancellationToken)
    {
        var specificationResult = await new ReceiptNumberMustBeUnique(request.Number, repository)
            .IsSatisfiedAsync(cancellationToken);
        if (specificationResult.IsFailure) return Result.Failure<ReceiptId>(specificationResult.Error);
        
        var receiptResult = Receipt.Create(request.Number, request.Date);
        if (receiptResult.IsFailure) return Result.Failure<ReceiptId>(receiptResult.Error);

        repository.Add(receiptResult.Value);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(receiptResult.Value.Id);
    }
}