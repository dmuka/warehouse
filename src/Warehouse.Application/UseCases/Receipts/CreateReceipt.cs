using MediatR;
using Warehouse.Application.UseCases.Receipts.Specifications;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Application.UseCases.Receipts;

public record CreateReceiptCommand(
    string Number,
    DateTime Date,
    IList<ReceiptItemRequest> Items) : IRequest<Result<ReceiptId>>;

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
        if (specificationResult.IsFailure) 
            return Result.Failure<ReceiptId>(specificationResult.Error);

        var receiptId = Guid.CreateVersion7();
        var receiptResult = Receipt.Create(
            request.Number, 
            request.Date,
            request.Items.Select(i => 
                ReceiptItem.Create(receiptId, i.ResourceId, i.UnitId, i.Quantity).Value).ToList(),
            receiptId);
    
        if (receiptResult.IsFailure) 
            return Result.Failure<ReceiptId>(receiptResult.Error);

        repository.Add(receiptResult.Value);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(receiptResult.Value.Id);
    }
}