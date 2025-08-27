using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Balances.Specification;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Receipts.DomainEvents;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Balances.Events;

public sealed class UpdateBalancesOnReceiptCreatedHandler(
    IWarehouseDbContext context,
    IResourceRepository resourceRepository,
    IUnitRepository unitRepository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator,
    ILogger<UpdateBalancesOnReceiptCreatedHandler> logger)
    : INotificationHandler<ReceiptCreatedDomainEvent>
{
    public async Task Handle(
        ReceiptCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        try
        {
            var resourceIds = notification.Items.Select(i => i.ResourceId).Distinct().ToList();
            var unitIds = notification.Items.Select(i => i.UnitId).Distinct().ToList();

            foreach (var resourceId in resourceIds)
            {
                var resourceSpecResult = await new ResourceMustExist(resourceId, resourceRepository)
                    .IsSatisfiedAsync(cancellationToken);
                if (resourceSpecResult.IsFailure)
                    throw new ApplicationException($"Resource validation failed: {resourceSpecResult.Error.Description}");
            }
            
            foreach (var unitId in unitIds)
            {
                var unitSpecResult = await new UnitMustExist(unitId, unitRepository)
                    .IsSatisfiedAsync(cancellationToken);
                if (unitSpecResult.IsFailure)
                    throw new ApplicationException($"Unit validation failed: {unitSpecResult.Error.Description}");
            }
            
            var existingBalances = await context.Balances
                .Where(b => resourceIds.Contains(b.ResourceId) && unitIds.Contains(b.UnitId))
                .ToListAsync(cancellationToken);

            var balanceDict = existingBalances
                .ToDictionary(b => (b.ResourceId, b.UnitId));

            foreach (var item in notification.Items)
            {
                var key = (new ResourceId(item.ResourceId), new UnitId(item.UnitId));
                
                if (balanceDict.TryGetValue(key, out var balance))
                {
                    var increaseResult = balance.Increase(item.Quantity);
                    if (increaseResult.IsFailure)
                        throw new ApplicationException($"Balance update failed: {increaseResult.Error.Description}");

                    context.Balances.Update(balance);
                    cache.Remove(keyGenerator.ForEntity<Balance>(balance.Id));
                    cache.RemoveAllForEntity<Balance>(balance.Id);
                }
                else
                {
                    var newBalanceResult = Balance.Create(
                        new ResourceId(item.ResourceId),
                        new UnitId(item.UnitId),
                        item.Quantity);
                    
                    if (newBalanceResult.IsFailure)
                        throw new ApplicationException($"Balance creation failed: {newBalanceResult.Error.Description}");
                    
                    var newBalance = newBalanceResult.Value;
                    context.Balances.Add(newBalance);
                    balanceDict[key] = newBalance;
                }
            }
            
            await context.SaveChangesAsync(cancellationToken);
            
            cache.Remove(keyGenerator.ForMethod<Balance>(nameof(GetFilteredBalancesQueryHandler)));
            cache.Remove(keyGenerator.ForMethod<Balance>(nameof(GetBalancesQueryHandler)));
            
            logger.LogInformation("Successfully updated balances for receipt {ReceiptId}", notification.ReceiptId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update balances for receipt {ReceiptId}", notification.ReceiptId);
            throw;
        }
    }
}