using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.UseCases.Receipts;
using Warehouse.Infrastructure.Data.DTOs;
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
    
    [HttpPost]
    [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create([FromBody] ReceiptDto dto)
    {
        var command = new CreateReceiptCommand(dto.ReceiptNumber, dto.ReceiptDate, dto.Items);
        
        var result = await mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}