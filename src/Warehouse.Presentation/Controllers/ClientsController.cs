using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.UseCases.Clients;
using Warehouse.Infrastructure.Data.DTOs;
using Warehouse.Presentation.Extensions;
using Warehouse.Presentation.Infrastructure;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create([FromBody] ClientDto resourceDto)
    {
        var command = new CreateClientCommand(resourceDto);
        
        var result = await mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpGet("{id:Guid}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById(Guid id)
    {
        var query = new GetClientByIdQuery(id);
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPatch("{id:Guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Archive(Guid id)
    {
        var command = new ArchiveClientCommand(id);
        
        var result = await mediator.Send(command);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
}