using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.UseCases.Units;
using Warehouse.Infrastructure.Data.DTOs;
using Warehouse.Presentation.Extensions;
using Warehouse.Presentation.Infrastructure;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UnitsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IList<UnitDto>), StatusCodes.Status200OK)]
    public async Task<IResult> GetUnits()
    {
        var query = new GetUnitsQuery();
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(UnitDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create([FromBody] UnitDto resourceDto)
    {
        var command = new CreateUnit(resourceDto);
        
        var result = await mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpGet("{id:Guid}")]
    [ProducesResponseType(typeof(UnitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById(Guid id)
    {
        var query = new GetUnitByIdQuery(id);
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpDelete("delete/{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> RemoveById(Guid id)
    {
        var query = new RemoveUnitByIdQuery(id);
        
        var result = await mediator.Send(query);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
    
    [HttpPatch("archive/{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Archive(Guid id)
    {
        var command = new ArchiveUnitCommand(id);
        
        var result = await mediator.Send(command);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
    
    [HttpPatch("unarchive/{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Unarchive(Guid id)
    {
        var command = new UnarchiveUnitCommand(id);
        
        var result = await mediator.Send(command);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
}