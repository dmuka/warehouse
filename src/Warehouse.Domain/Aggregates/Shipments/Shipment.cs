using System.ComponentModel.DataAnnotations;
using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Domain.Aggregates.Resources;
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
    private readonly List<ShipmentItem> _items = [];

    protected Shipment() { }

    private Shipment(
        string number,
        DateTime date,
        ClientId clientId,
        ShipmentId shipmentId)
    {
        Number = number;
        Date = date;
        ClientId = clientId;
        Id = shipmentId;
        Status = ShipmentStatus.Draft;
    }

    public static Result<Shipment> Create(
        string number,
        DateTime date,
        Guid clientId,
        Guid? shipmentId = null)
    {
        var validationResults = ValidateShipmentDetails(number);
        if (validationResults.Length != 0)
            return Result<Shipment>.ValidationFailure(ValidationError.FromResults(validationResults));

        return new Shipment(
            number,
            date,
            new ClientId(clientId),
            shipmentId is null ? new ShipmentId(Guid.NewGuid()) : new ShipmentId(shipmentId.Value));
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
    
    public Result ChangeStatus(ShipmentStatus newStatus)
    {
        var transitionResult = Status.CanTransitionTo(newStatus);
        if (transitionResult.IsFailure) return transitionResult;

        if (newStatus == ShipmentStatus.Signed)
        {
            if (_items.Count == 0) return Result.Failure(ShipmentErrors.EmptyShipment);
        }

        Status = newStatus;
        
        return Result.Success();
    }

    public Result Cancel(string reason)
    {
        if (Status.IsFinal()) return Result.Failure(ShipmentErrors.AlreadyFinalized(Id, reason));

        var result = ChangeStatus(ShipmentStatus.Cancelled);
        
        return result;
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