using System.Net;
using System.Text.Json;
using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace SampleApi;

public class SampleApi
{
    private readonly ILogger _logger;
    private readonly InMemoryDataService _service;

    public SampleApi(ILoggerFactory loggerFactory,
        InMemoryDataService service)
    {
        _logger = loggerFactory.CreateLogger<SampleApi>();
        _service = service;
    }

    [Function("SampleGet")]
    public HttpResponseData Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "sample/{id}")] HttpRequestData req, int id)
    {
        ValidateId(id);
        var item = _service.Get(id);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString(JsonSerializer.Serialize(item));

        return response;
    }


    [Function("SamplePost")]
    public async Task<HttpResponseData> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "sample")] HttpRequestData req)
    {
        var label = await req.ReadAsStringAsync();
        if (string.IsNullOrEmpty(label))
        {
            throw new ArgumentException("No label provided");
        };

        _service.Add(label);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString(JsonSerializer.Serialize(new { Status = (int)HttpStatusCode.OK, Message = "Added" }));

        return response;
    }

    [Function("SamplePatch")]
    public async Task<HttpResponseData> Patch([HttpTrigger(AuthorizationLevel.Function, "patch", Route = "sample/{id}")] HttpRequestData req, int id)
    {
        ValidateId(id);
        var label = await req.ReadAsStringAsync();

        if (string.IsNullOrEmpty(label))
        {
            throw new ArgumentException("No label provided");
        };

        _service.Update(id, label);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString(JsonSerializer.Serialize(new { Status = (int)HttpStatusCode.OK, Message = "Patched" }));

        return response;
    }

    [Function("SampleDelete")]
    public HttpResponseData Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "sample/{id}")] HttpRequestData req, int id)
    {
        ValidateId(id);
        _service.Remove(id);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString(JsonSerializer.Serialize(new { Status = (int)HttpStatusCode.OK, Message = "Removed" }));

        return response;
    }

    private void ValidateId(int id)
    {
        if (id < 0)
        {
            throw new ArgumentException("Id must not be less than zero", nameof(id));
        }
    }
}
