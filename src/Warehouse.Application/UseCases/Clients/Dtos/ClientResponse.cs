namespace Warehouse.Application.UseCases.Clients.Dtos;

public sealed record ClientResponse(Guid Id, string ClientName, string ClientAddress, bool IsActive);