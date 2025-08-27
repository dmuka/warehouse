using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Receipts.DomainEvents;

namespace Warehouse.Application.UseCases.Balances.Events;

public sealed class UpdateBalancesOnReceiptRemovedHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator,
    ILogger<UpdateBalancesOnReceiptRemovedHandler> logger)
    : INotificationHandler<ReceiptRemovedDomainEvent>
{
    public async Task Handle(
        ReceiptRemovedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        try
        {
            var resourceIds = notification.Items.Select(i => i.ResourceId).Distinct().ToList();
            var unitIds = notification.Items.Select(i => i.UnitId).Distinct().ToList();

            var balances = await context.Balances
                .Where(b => resourceIds.Contains(b.ResourceId) && unitIds.Contains(b.UnitId))
                .ToListAsync(cancellationToken);

            var balanceDict = balances
                .GroupBy(b => (b.ResourceId, b.UnitId))
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var item in notification.Items)
            {
                var key = (item.ResourceId, item.UnitId);
                
                if (!balanceDict.TryGetValue(key, out var balance))
                {
                    throw new InvalidOperationException(
                        $"Balance not found for ResourceId: {item.ResourceId}, UnitId: {item.UnitId}");
                }
                    
                var result = balance.Decrease(item.Quantity);
                if (result.IsFailure)
                {
                    throw new InvalidOperationException(
                        $"Failed to decrease balance: {result.Error.Description}");
                }

                context.Balances.Update(balance);
                await context.SaveChangesAsync(cancellationToken);
                
                cache.Remove(keyGenerator.ForEntity<Balance>(balance.Id));
                cache.Remove(keyGenerator.ForMethod<Balance>(nameof(GetBalancesQueryHandler)));
                cache.Remove(keyGenerator.ForMethod<Balance>(nameof(GetFilteredBalancesQuery)));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update balances for removed receipt {ReceiptId}", 
                notification.ReceiptId);
            throw;
        }
    }
}