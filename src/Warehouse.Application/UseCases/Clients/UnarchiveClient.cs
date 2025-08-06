using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Application.UseCases.Clients;

public record UnarchiveClientCommand(Guid Id) : IRequest<Result>;

public sealed class UnarchiveClientCommandHandler(
    IClientRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UnarchiveClientCommand, Result>
{
    public async Task<Result> Handle(UnarchiveClientCommand request, CancellationToken cancellationToken)
    {
        var client = await repository.GetByIdAsync(new ClientId(request.Id), cancellationToken);
        if (client is null) return Result.Failure<Client>(ClientErrors.NotFound(request.Id));
        if (client.IsActive) return Result.Failure<Client>(ClientErrors.ClientAlreadyActive);
        
        client.Activate();

        repository.Update(client);
        await unitOfWork.CommitAsync(cancellationToken);
        
        return Result.Success();
    }
}