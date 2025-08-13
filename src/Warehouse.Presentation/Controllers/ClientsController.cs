using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.UseCases.Clients;
using Warehouse.Application.UseCases.Clients.Dtos;
using Warehouse.Infrastructure.Data.DTOs;
using Warehouse.Presentation.Extensions;
using Warehouse.Presentation.Infrastructure;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IList<ClientResponse>), StatusCodes.Status200OK)]
    public async Task<IResult> GetClients()
    {
        var query = new GetClientsQuery();
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create([FromBody] ClientRequest clientDto)
    {
        var command = new CreateClientCommand(clientDto);
        
        var result = await mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Update([FromBody] ClientRequest clientDto)
    {
        var command = new UpdateClientCommand(clientDto);
        
        var result = await mediator.Send(command);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
    
    [HttpGet("{id:Guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById(Guid id)
    {
        var query = new GetClientByIdQuery(id);
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpDelete("delete/{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> RemoveById(Guid id)
    {
        var query = new RemoveClientByIdQuery(id);
        
        var result = await mediator.Send(query);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
    
    [HttpPatch("archive/{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Archive(Guid id)
    {
        var command = new ArchiveClientCommand(id);
        
        var result = await mediator.Send(command);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
    
    [HttpPatch("unarchive/{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Unarchive(Guid id)
    {
        var command = new UnarchiveClientCommand(id);
        
        var result = await mediator.Send(command);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
}