namespace Warehouse.Application.UseCases.Clients.Dtos;

public sealed record ClientRequest(Guid Id, string ClientName, string ClientAddress, bool IsActive);