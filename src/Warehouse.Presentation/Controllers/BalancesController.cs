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
    [ProducesResponseType(typeof(BalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetBalance(GetBalanceDto dto)
    {
        var query = new GetBalanceQuery(dto.ResourceId, dto.UnitId);
        
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