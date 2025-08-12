using MediatR;

namespace Warehouse.Core;
/// <summary>
/// Represents a domain event, which encapsulates a significant occurrence or state change within the domain model.
/// </summary>
public interface IDomainEvent : INotification;