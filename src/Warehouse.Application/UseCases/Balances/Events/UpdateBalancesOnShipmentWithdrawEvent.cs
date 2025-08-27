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

public sealed class UpdateBalancesOnShipmentWithdrawedHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator,
    ILogger<UpdateBalancesOnShipmentWithdrawedHandler> logger) : INotificationHandler<ShipmentWithdrawedDomainEvent>
{
    public async Task Handle(
        ShipmentWithdrawedDomainEvent notification,
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
                
                var result = balance.Increase(item.Quantity);
                if (result.IsFailure) throw new InvalidOperationException(result.Error.Description);

                context.Balances.Update(balance);
                await context.SaveChangesAsync(cancellationToken);
                cache.Remove(keyGenerator.ForMethod<Balance>(nameof(GetBalancesQueryHandler)));
                cache.Remove(keyGenerator.ForEntity<Balance>(item.Id));
                
                
                cache.RemoveAllForEntity<Balance>(balance.Id);

                var cacheKey = keyGenerator.ForMethod<Balance>(nameof(GetAvailableByResourceAndUnitQueryHandler),
                    (nameof(ResourceId), item.ResourceId.Value),
                    (nameof(UnitId), item.UnitId.Value));
                logger.LogDebug("Removing cache key: {CacheKey}", cacheKey);
                cache.Remove(cacheKey);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update balances for shipment {ShipmentId}", notification.ShipmentId);
            throw;
        }
    }
}