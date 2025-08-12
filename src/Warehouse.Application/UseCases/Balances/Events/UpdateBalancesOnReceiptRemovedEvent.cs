using MediatR;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Receipts.DomainEvents;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Balances.Events;

public sealed class UpdateBalancesOnReceiptRemovedHandler(
    IBalanceRepository balanceRepository,
    IUnitOfWork unitOfWork)
    : INotificationHandler<ReceiptRemovedDomainEvent>
{
    public async Task Handle(
        ReceiptRemovedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            foreach (var item in notification.Items)
            {
                var balance = await balanceRepository.GetByResourceAndUnitAsync(
                    new ResourceId(item.ResourceId),
                    new UnitId(item.UnitId),
                    cancellationToken);
                if (balance is null) throw new InvalidOperationException("Balance not found.");
                
                var result = balance.Decrease(item.Quantity);
                if (result.IsFailure) throw new InvalidOperationException(result.Error.Description);

                balanceRepository.Update(balance);
            }

            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}