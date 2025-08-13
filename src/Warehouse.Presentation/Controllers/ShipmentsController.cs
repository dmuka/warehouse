using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.UseCases.Shipments;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Presentation.DTOs;
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

    [HttpPost("filter")]
    [ProducesResponseType(typeof(IList<ShipmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetFiltered([FromBody] ShipmentFilterDto filter)
    {
        var query = new GetFilteredShipmentsQuery(
            filter.DateFrom, 
            filter.DateTo, 
            filter.ReceiptNumber,
            filter.ClientIds,
            filter.ResourceIds, 
            filter.UnitIds);
        
        var result = await mediator.Send(query);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ShipmentRequest), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create(ShipmentCreateRequest request)
    {
        var command = new CreateShipmentCommand(request);
        
        var result = await mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpGet("{id:Guid}")]
    [ProducesResponseType(typeof(ShipmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById(Guid id)
    {
        var command = new GetShipmentByIdQuery(id);
        
        var result = await mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpDelete("delete/{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteById(Guid id)
    {
        var command = new RemoveShipmentByIdQuery(id);
        
        var result = await mediator.Send(command);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
    
    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Update(ShipmentRequest shipmentRequest)
    {
        var command = new UpdateShipmentCommand(shipmentRequest);
        
        var result = await mediator.Send(command);

        return result.Match(Results.NoContent, CustomResults.Problem);
    }
}