using MediatR;
using Warehouse.Application.UseCases.Balances.Specification;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Receipts.DomainEvents;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Balances.Events;

public sealed class UpdateBalancesOnReceiptCreatedHandler(
    IBalanceRepository balanceRepository,
    IResourceRepository resourceRepository,
    IUnitRepository unitRepository,
    IUnitOfWork unitOfWork)
    : INotificationHandler<ReceiptCreatedDomainEvent>
{
    public async Task Handle(
        ReceiptCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in notification.Items)
            {
                var resourceSpecResult = await new ResourceMustExist(item.ResourceId, resourceRepository)
                    .IsSatisfiedAsync(cancellationToken);
                if (resourceSpecResult.IsFailure) throw new ApplicationException(resourceSpecResult.Error.Description);

                var unitSpecResult = await new UnitMustExist(item.UnitId, unitRepository)
                    .IsSatisfiedAsync(cancellationToken);
                if (unitSpecResult.IsFailure) throw new ApplicationException(unitSpecResult.Error.Description);

                var balance = await balanceRepository.GetByResourceAndUnitAsync(
                    new ResourceId(item.ResourceId),
                    new UnitId(item.UnitId),
                    cancellationToken);

                if (balance is null)
                {
                    var newBalanceResult = Balance.Create(
                        new ResourceId(item.ResourceId),
                        new UnitId(item.UnitId),
                        item.Quantity);
                    if (newBalanceResult.IsFailure) throw new ApplicationException(newBalanceResult.Error.Description);
                    
                    balance = newBalanceResult.Value;
                    balanceRepository.Add(balance);
                }
                else
                {
                    var updateResult = balance.Increase(item.Quantity);
                    if (updateResult.IsFailure) throw new ApplicationException(updateResult.Error.Description);
                    
                    balanceRepository.Update(balance);
                }
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