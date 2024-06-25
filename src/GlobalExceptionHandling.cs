using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace SampleApi;

public class GlobalExceptionHandling : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (AggregateException aggregateException)
        {
            var req = await context.GetHttpRequestDataAsync();
            var res = req!.CreateResponse();
            res.StatusCode = HttpStatusCode.BadRequest;
            res.WriteString(JsonSerializer.Serialize(new { res.StatusCode, Message = string.Join(", ", aggregateException.InnerExceptions.Select(x => x.Message)) }));
            context.GetInvocationResult().Value = res;
        }
        catch (ArgumentOutOfRangeException argumentOutOfRangeException)
        {
            var req = await context.GetHttpRequestDataAsync();
            var res = req!.CreateResponse();
            res.StatusCode = HttpStatusCode.BadRequest;
            res.WriteString(JsonSerializer.Serialize(new { res.StatusCode, Message = argumentOutOfRangeException.Message }));
            context.GetInvocationResult().Value = res;
        }
        catch (Exception exception)
        {
            var req = await context.GetHttpRequestDataAsync();
            var res = req!.CreateResponse();
            res.StatusCode = HttpStatusCode.InternalServerError;
            res.WriteString(JsonSerializer.Serialize(new { res.StatusCode, exception.Message}));
            context.GetInvocationResult().Value = res;
        }
    }
}
