using Microsoft.AspNetCore.Mvc.Filters;
using CashFlow.Exception.ExceptionsBase;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.API.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is CashFlowException)
        {
            HandleProjectException(context);
        }
        else
        {
            ThrowUnknownError(context);
        }
    }

    private void HandleProjectException(ExceptionContext context)
    {
        var ex = (ErrorOnValidationException)context.Exception;

        var errorResponse = new ResponseErrorJson(ex.Errors);

        context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        context.Result = new ObjectResult(errorResponse);
    }

    private void ThrowUnknownError(ExceptionContext context)
    {
        var errorResponse = new ResponseErrorJson("An unexpected error occurred.");

        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        context.Result = new ObjectResult(errorResponse);
    }
}
