using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Shipments.DomainEvents;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Balances.Events;

public sealed class UpdateBalancesOnShipmentSignedHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator,
    ILogger<UpdateBalancesOnShipmentSignedHandler> logger) : INotificationHandler<ShipmentSignedDomainEvent>
{
    public async Task Handle(
        ShipmentSignedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        try
        {
            foreach (var item in notification.Items)
            {
                var balance = await context.Balances.FirstOrDefaultAsync(balance =>
                    balance.ResourceId == item.ResourceId
                    && balance.UnitId == item.UnitId, cancellationToken);
                if (balance is null) throw new InvalidOperationException("Balance not found.");
                
                var updateResult = balance.Decrease(item.Quantity);
                if (updateResult.IsFailure) throw new ApplicationException(updateResult.Error.Description);
                
                context.Balances.Update(balance);
                await context.SaveChangesAsync(cancellationToken);
                cache.Remove(keyGenerator.ForMethod<Balance>(nameof(GetBalancesQueryHandler)));
                cache.Remove(keyGenerator.ForEntity<Balance>(item.Id));
                
                
                cache.RemoveAllForEntity<Balance>(balance.Id);
                cache.Remove(keyGenerator.ForMethod<Balance>(nameof(GetAvailableByResourceAndUnitQueryHandler), 
                    (nameof(ResourceId), item.ResourceId.Value), 
                    (nameof(UnitId), item.UnitId.Value)));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update balances for shipment {ShipmentId}", notification.ShipmentId);
            throw;
        }
    }
}