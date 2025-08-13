using MediatR;
using Warehouse.Application.UseCases.Resources.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record GetResourceByIdQuery(Guid Id) : IRequest<Result<ResourceResponse>>;

public sealed class GetResourceByIdQueryHandler(IResourceRepository repository) : IRequestHandler<GetResourceByIdQuery, Result<ResourceResponse>>
{
    public async Task<Result<ResourceResponse>> Handle(GetResourceByIdQuery request, CancellationToken cancellationToken)
    {
        var id = new ResourceId(request.Id);
        var resource = await repository.GetByIdAsync(id, cancellationToken);;
        if (resource is null) return Result.Failure<ResourceResponse>(ResourceErrors.NotFound(request.Id));
        
        var response = new ResourceResponse(resource.Id.Value, resource.ResourceName.Value, resource.IsActive);

        return response;
    }
}