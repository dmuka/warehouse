using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Receipts.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Application.UseCases.Receipts;

public record UpdateReceiptCommand(ReceiptRequest ReceiptRequest) : IRequest<Result>;

public sealed class UpdateReceiptCommandHandler(
    IReceiptRepository receiptRepository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateReceiptCommand, Result>
{
    public async Task<Result> Handle(
        UpdateReceiptCommand request,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        
        var receipt = await receiptRepository.GetByIdAsync(new ReceiptId(request.ReceiptRequest.Id), true, cancellationToken);
        if (receipt is null) return Result.Failure(ReceiptErrors.NotFound(request.ReceiptRequest.Id));

        receipt.Update(
            request.ReceiptRequest.ReceiptNumber, 
            request.ReceiptRequest.ReceiptDate,
            request.ReceiptRequest.Items.Select(i => 
                ReceiptItem.Create(receipt.Id, i.ResourceId, i.UnitId, i.Quantity, i.Id).Value).ToList());
        
        try
        {
            receiptRepository.Update(receipt);
            await unitOfWork.CommitAsync(cancellationToken);
            cache.Remove(keyGenerator.ForMethod<Receipt>(nameof(GetReceiptsQueryHandler)));
            cache.RemoveAllForEntity<Receipt>(receipt.Id);
            
            return Result.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}