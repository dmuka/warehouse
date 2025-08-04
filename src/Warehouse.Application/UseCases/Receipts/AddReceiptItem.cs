using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Receipts;

public record AddReceiptItemCommand(
    Guid ReceiptId,
    Guid ResourceId,
    Guid UnitId,
    decimal Quantity) : IRequest<Result>;

public sealed class AddReceiptItemCommandHandler(
    IReceiptRepository receiptRepository,
    IResourceRepository resourceRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AddReceiptItemCommand, Result>
{
    public async Task<Result> Handle(
        AddReceiptItemCommand request,
        CancellationToken cancellationToken)
    {
        var receipt = await receiptRepository.GetByIdAsync(
            new ReceiptId(request.ReceiptId),
            includeItems: true,
            cancellationToken);

        if (receipt is null)
            return Result.Failure(ReceiptErrors.NotFound(request.ReceiptId));

        var result = receipt.AddItem(
            new ResourceId(request.ResourceId),
            new UnitId(request.UnitId),
            request.Quantity);

        if (result.IsFailure)
            return result;

        receiptRepository.Update(receipt);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}