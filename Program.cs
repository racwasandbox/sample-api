using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleApi;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder => {
        builder.UseMiddleware<GlobalExceptionHandling>();
        builder.UseNewtonsoftJson();
    })
    .ConfigureServices(serviceCollection => {
        serviceCollection.AddSingleton<InMemoryDataService>();
    })
    .Build();

host.Run();
