using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Application.UseCases.Clients;

public record RemoveClientByIdQuery(Guid Id) : IRequest<Result>;

public sealed class RemoveClientByIdQueryHandler(IClientRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<RemoveClientByIdQuery, Result>
{
    public async Task<Result> Handle(RemoveClientByIdQuery request, CancellationToken cancellationToken)
    {
        var id = new ClientId(request.Id);
        var isClientExist = await repository.ExistsByIdAsync(id, cancellationToken);
        if (!isClientExist) return Result.Failure<Client>(ClientErrors.NotFound(request.Id));

        await repository.Delete(id);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}