using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record GetResourceByIdQuery(Guid Id) : IRequest<Result<Resource>>;

public sealed class GetResourceByIdQueryHandler(IResourceRepository repository) : IRequestHandler<GetResourceByIdQuery, Result<Resource>>
{
    public async Task<Result<Resource>> Handle(GetResourceByIdQuery request, CancellationToken cancellationToken)
    {
        var id = new ResourceId(request.Id);
        var isResourceExist = await repository.ExistsByIdAsync(id, cancellationToken);
        if (!isResourceExist) return Result.Failure<Resource>(ResourceErrors.NotFound(request.Id));

        var resource = await repository.GetByIdAsync(id, cancellationToken);

        return resource;
    }
}