using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record GetResourcesQuery : IRequest<Result<IList<Resource>>>;

public sealed class GetBalancesQueryHandler(
    IResourceRepository clientRepository) : IRequestHandler<GetResourcesQuery, Result<IList<Resource>>>
{
    public async Task<Result<IList<Resource>>> Handle(
        GetResourcesQuery request,
        CancellationToken cancellationToken)
    {
        var resources = await clientRepository.GetListAsync(cancellationToken);

        return Result.Success(resources);
    }
}