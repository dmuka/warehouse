using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.UseCases.Receipts;
using Warehouse.Application.UseCases.Receipts.Dtos;
using Warehouse.Presentation.DTOs;
using Warehouse.Presentation.Extensions;
using Warehouse.Presentation.Infrastructure;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReceiptsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IList<ReceiptResponse>), StatusCodes.Status200OK)]
    public async Task<IResult> GetReceipts()
    {
        var query = new GetReceiptsQuery();
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("filter")]
    [ProducesResponseType(typeof(IList<ReceiptResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetFiltered([FromBody] ReceiptFilterDto filter)
    {
        var query = new GetFilteredReceiptsQuery(
            filter.DateFrom, 
            filter.DateTo, 
            filter.ReceiptNumber, 
            filter.ResourceIds, 
            filter.UnitIds);
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ReceiptRequest), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create([FromBody] ReceiptRequest request)
    {
        var command = new CreateReceiptCommand(request);
        
        var result = await mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpGet("{id:Guid}")]
    [ProducesResponseType(typeof(ReceiptResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById(Guid id)
    {
        var command = new GetReceiptByIdQuery(id);
        
        var result = await mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpDelete("delete/{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteById(Guid id)
    {
        var command = new RemoveReceiptByIdQuery(id);
        
        var result = await mediator.Send(command);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
}