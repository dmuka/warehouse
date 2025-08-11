using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.UseCases.Balances;
using Warehouse.Infrastructure.Data.DTOs;
using Warehouse.Presentation.DTOs;
using Warehouse.Presentation.Extensions;
using Warehouse.Presentation.Infrastructure;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BalancesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IList<BalanceDto2>), StatusCodes.Status200OK)]
    public async Task<IResult> GetBalances()
    {
        var query = new GetBalancesQuery();
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("filter")]
    [ProducesResponseType(typeof(IList<BalanceDto2>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetFiltered([FromBody] BalanceFilterDto filter)
    {
        var query = new GetFilteredBalancesQuery(filter.ResourceNames, filter.UnitNames);
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("available")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetFiltered([FromBody] AvailableDto dto)
    {
        var query = new GetAvailableByResourceAndUnitQuery(dto.ResourceId, dto.UnitId);
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpGet("{resourceId:Guid}/{unitId:Guid}")]
    [ProducesResponseType(typeof(BalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetBalance(Guid resourceId, Guid unitId)
    {
        var query = new GetBalanceQuery(resourceId, unitId);
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> UpdateBalance(UpdateBalanceDto dto)
    {
        var command = new UpdateBalanceCommand(dto.ResourceId, dto.UnitId, dto.Quantity);
        
        var result = await mediator.Send(command);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
}