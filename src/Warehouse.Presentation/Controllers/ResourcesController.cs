using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.UseCases.Resources;
using Warehouse.Infrastructure.Data.DTOs;
using Warehouse.Presentation.Extensions;
using Warehouse.Presentation.Infrastructure;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ResourcesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IList<ResourceDto>), StatusCodes.Status200OK)]
    public async Task<IResult> GetResources()
    {
        var query = new GetResourcesQuery();
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create([FromBody] ResourceDto resourceDto)
    {
        var command = new CreateResourceCommand(resourceDto);
        
        var result = await mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpGet("{id:Guid}")]
    [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById(Guid id)
    {
        var query = new GetResourceByIdQuery(id);
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPatch("{id:Guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Archive(Guid id)
    {
        var command = new ArchiveResourceCommand(id);
        
        var result = await mediator.Send(command);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
}