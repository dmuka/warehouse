namespace Warehouse.Application.UseCases.Resources.Dtos;

public sealed record ResourceRequest(Guid Id, string ResourceName, bool IsActive);