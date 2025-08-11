using MediatR;
using Warehouse.Application.UseCases.Resources.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record GetResourcesQuery : IRequest<Result<IList<ResourceResponse>>>;

public sealed class GetBalancesQueryHandler(
    IResourceRepository clientRepository) : IRequestHandler<GetResourcesQuery, Result<IList<ResourceResponse>>>
{
    public async Task<Result<IList<ResourceResponse>>> Handle(
        GetResourcesQuery request,
        CancellationToken cancellationToken)
    {
        var resources = await clientRepository.GetListAsync(cancellationToken);

        var response = resources
            .Select(resource => new ResourceResponse(resource.Id.Value, resource.ResourceName.Value, resource.IsActive))
            .ToList(); 

        return Result.Success<IList<ResourceResponse>>(response);
    }
}