namespace Warehouse.Application.UseCases.Resources.Dtos;

public sealed record ResourceResponse(Guid Id, string ResourceName, bool IsActive);