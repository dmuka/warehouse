using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.UseCases.Shipments;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Presentation.Extensions;
using Warehouse.Presentation.Infrastructure;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ShipmentsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IList<ShipmentResponse>), StatusCodes.Status200OK)]
    public async Task<IResult> GetShipments()
    {
        var query = new GetShipmentsQuery();
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ShipmentRequest), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create([FromBody] ShipmentRequest dto)
    {
        var command = new CreateShipmentCommand(dto);
        
        var result = await mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}