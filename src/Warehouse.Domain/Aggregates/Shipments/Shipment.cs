using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Shipments.DomainEvents;
using Warehouse.Domain.Aggregates.Shipments.Specifications;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Domain.Aggregates.Shipments;

public class Shipment : AggregateRoot
{
    [Key]
    public new ShipmentId Id { get; protected set; } = null!;
    public string Number { get; private set; } = null!;
    public DateTime Date { get; private set; }
    public ClientId ClientId { get; private set; } = null!;
    public ShipmentStatus Status { get; private set; }
    public IReadOnlyCollection<ShipmentItem> Items => _items.AsReadOnly();
    private IList<ShipmentItem> _items = [];

    protected Shipment() { }

    private Shipment(
        string number,
        DateTime date,
        ClientId clientId, 
        IList<ShipmentItem> items,
        ShipmentId shipmentId)
    {
        Number = number;
        Date = date;
        ClientId = clientId;
        Id = shipmentId;
        _items = items;
        Status = ShipmentStatus.Draft;
    }

    public static Result<Shipment> Create(
        string number,
        DateTime date,
        Guid clientId,
        IList<ShipmentItem> items,
        ShipmentStatus status = ShipmentStatus.Draft,
        Guid? shipmentId = null)
    {
        var validationResults = ValidateShipmentDetails(number);
        if (validationResults.Length != 0)
            return Result<Shipment>.ValidationFailure(ValidationError.FromResults(validationResults));
        if (status == ShipmentStatus.Signed && items.Count == 0)
            return Result.Failure<Shipment>(ShipmentErrors.EmptyShipment);
        
        var shipment = new Shipment(
            number,
            date,
            new ClientId(clientId),
            items,
            shipmentId is null ? new ShipmentId(Guid.NewGuid()) : new ShipmentId(shipmentId.Value));
    
        if (status == ShipmentStatus.Signed) shipment.ChangeStatus(ShipmentStatus.Signed);
        
        shipment.AddDomainEvent(new ShipmentCreatedDomainEvent(
            shipment.Id,
            items.Select(i => ShipmentItem.Create(
                shipment.Id,
                i.ResourceId.Value,
                i.UnitId.Value,
                i.Quantity,
                i.Id).Value).ToList()));

        return shipment;
    }

    public Result<Shipment> Update(
        string number,
        DateTime date,
        Guid clientId,
        IList<ShipmentItem> items,
        ShipmentStatus status )
    {
        var validationResults = ValidateShipmentDetails(number);
        if (validationResults.Length != 0)
            return Result<Shipment>.ValidationFailure(ValidationError.FromResults(validationResults));
        if (status == ShipmentStatus.Signed && items.Count == 0)
            return Result.Failure<Shipment>(ShipmentErrors.EmptyShipment);

        Number = number;
        Date = date;
        ClientId = new ClientId(clientId);
        
        var itemsToRemove = _items.Except(items).ToList();
        var itemsToAdd = items.Except(_items).ToList();
    
        foreach (var item in itemsToRemove)
        {
            _items.Remove(item);
        }
    
        foreach (var item in itemsToAdd)
        {
            _items.Add(item);
        }

        if (status == ShipmentStatus.Signed) return this;
        
        ChangeStatus(ShipmentStatus.Signed);
        AddDomainEvent(new ShipmentSignedDomainEvent(
            Id,
            items.Select(i => ShipmentItem.Create(
                Id,
                i.ResourceId.Value,
                i.UnitId.Value,
                i.Quantity,
                i.Id).Value).ToList()));

        return this;
    }
    
    public void Remove()
    {
        AddDomainEvent(new ShipmentRemovedDomainEvent(
            Id,
            Items.ToList()));
    }

    public Result AddItem(ResourceId resourceId, UnitId unitId, decimal quantity)
    {
        var itemResult = ShipmentItem.Create(Id, resourceId, unitId, quantity);
        if (itemResult.IsFailure) return itemResult;

        if (_items.Any(i => i.ResourceId == resourceId && i.UnitId == unitId))
            return Result.Failure(ShipmentErrors.ShipmentItemAlreadyExist(resourceId, unitId));

        _items.Add(itemResult.Value);
        
        return Result.Success();
    }
    
    private Result CanTransitionTo(ShipmentStatus newStatus)
    {
        switch (Status)
        {
            case ShipmentStatus.Draft:
                if (newStatus is ShipmentStatus.Cancelled or ShipmentStatus.Signed)
                    return Result.Success();
                break;
                
            case ShipmentStatus.Signed:
                if (newStatus == ShipmentStatus.Cancelled)
                    return Result.Success();
                break;
                
            case ShipmentStatus.Cancelled:
                break;
        }

        return Result.Failure(ShipmentErrors.InvalidStatusTransition(Status, newStatus));
    }
    
    public Result ChangeStatus(ShipmentStatus newStatus)
    {
        var transitionResult = Status.CanTransitionTo(newStatus);
        if (transitionResult.IsFailure) return transitionResult;
        
        if (_items.Count == 0) return Result.Failure(ShipmentErrors.EmptyShipment);

        Status = newStatus;
        
        if (newStatus == ShipmentStatus.Signed) AddDomainEvent(new ShipmentSignedDomainEvent(Id, Items.ToList()));
        
        return Result.Success();
    }

    private static Result[] ValidateShipmentDetails(string number)
    {
        var results = new[]
        {
            new ShipmentNumberMustBeValid(number).IsSatisfied()
        };

        return results.Where(r => r.IsFailure).ToArray();
    }
}