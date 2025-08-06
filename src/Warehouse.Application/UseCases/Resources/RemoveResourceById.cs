using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources;

public record RemoveResourceByIdQuery(Guid Id) : IRequest<Result>;

public sealed class RemoveResourceByIdQueryHandler(IResourceRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<RemoveResourceByIdQuery, Result>
{
    public async Task<Result> Handle(RemoveResourceByIdQuery request, CancellationToken cancellationToken)
    {
        var id = new ResourceId(request.Id);
        var isResourceExist = await repository.ExistsByIdAsync(id, cancellationToken);
        if (!isResourceExist) return Result.Failure<Resource>(ResourceErrors.NotFound(request.Id));

        await repository.Delete(id);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}