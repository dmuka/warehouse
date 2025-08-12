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
        var client = await repository.GetByIdAsync(id, cancellationToken);
        if (client is null) return Result.Failure<Client>(ClientErrors.NotFound(request.Id));

        repository.Delete(client);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}