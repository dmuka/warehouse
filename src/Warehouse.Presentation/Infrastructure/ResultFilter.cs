using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Warehouse.Core.Results;

namespace Warehouse.Presentation.Infrastructure;

public class ResultFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult { Value: Result result })
        {
            if (result.IsFailure)
            {
                context.Result = result.Error.Type switch
                {
                    ErrorType.NotFound => new NotFoundObjectResult(result.Error),
                    ErrorType.Conflict => new ConflictObjectResult(result.Error),
                    _ => new BadRequestObjectResult(result.Error)
                };
            }
        }
        await next();
    }
}