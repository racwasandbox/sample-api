using System.Net;
using System.Text.Json;
using Grpc.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

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

    [OpenApiOperation(operationId: "health", tags: new[] { "health" }, Summary = "health", Description = "Gets the health of the api")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotAcceptable)]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound)]
    [OpenApiResponseWithoutBody(HttpStatusCode.TooManyRequests)]
    [Function("health")]
    public HttpResponseData Health([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/json; charset=utf-8");
        response.WriteString(JsonSerializer.Serialize(new { StatusCode.OK, Message = "Healthy" }));

        return response;
    }

    [OpenApiOperation(operationId: "get", tags: new[] { "get" }, Summary = "get", Description = "get")]
    [OpenApiParameter("id", Description = "The id of the item", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Item), Summary = "The response", Description = "This returns the response")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotAcceptable)]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound)]
    [OpenApiResponseWithoutBody(HttpStatusCode.TooManyRequests)]

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


    [OpenApiOperation(operationId: "post", tags: new[] { "post" }, Summary = "post", Description = "post")]
    [OpenApiRequestBody("text/plain", bodyType: typeof(string), Description = "The text to update the item with")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotAcceptable)]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound)]
    [OpenApiResponseWithoutBody(HttpStatusCode.TooManyRequests)]

    [Function("SamplePost")]
    public async Task<HttpResponseData> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "sample")] HttpRequestData req)
    {
        var label = await req.ReadAsStringAsync();
        if (string.IsNullOrEmpty(label))
        {
            throw new ArgumentException("No label provided");
        }

        _service.Add(label);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString(JsonSerializer.Serialize(new { Status = (int)HttpStatusCode.OK, Message = "Added" }));

        return response;
    }

    [OpenApiOperation(operationId: "patch", tags: new[] { "patch" }, Summary = "patch", Description = "patch")]
    [OpenApiParameter("id", Description = "The id of the item", Required = true)]
    [OpenApiRequestBody("text/plain", bodyType: typeof(string), Description = "The text to update the item with")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotAcceptable)]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound)]
    [OpenApiResponseWithoutBody(HttpStatusCode.TooManyRequests)]

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

    [OpenApiOperation(operationId: "delete", tags: new[] { "delete" }, Summary = "delete", Description = "delete")]
    [OpenApiParameter("id", Description = "The id of the item", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotAcceptable)]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound)]
    [OpenApiResponseWithoutBody(HttpStatusCode.TooManyRequests)]

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


    [Function("SampleDelete")]
    public HttpResponseData GetData([HttpTrigger(AuthorizationLevel.Anonymous, "getdata", Route = "sample/{id}")] HttpRequestData req, int id)
    {
        ValidateId(id);
        var item = _service.Get(id);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString(JsonSerializer.Serialize(item));

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
