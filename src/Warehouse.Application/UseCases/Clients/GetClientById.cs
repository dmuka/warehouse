using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Application.UseCases.Clients;

public record GetClientByIdQuery(Guid Id) : IRequest<Result<Client>>;

public sealed class GetClientByIdQueryHandler(IClientRepository repository) : IRequestHandler<GetClientByIdQuery, Result<Client>>
{
    public async Task<Result<Client>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var id = new ClientId(request.Id);
        var isClientExist = await repository.ExistsByIdAsync(id, cancellationToken);
        if (!isClientExist) return Result.Failure<Client>(ClientErrors.NotFound(request.Id));

        var client = await repository.GetByIdAsync(id, cancellationToken);

        return client;
    }
}