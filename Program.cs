using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleApi;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder => {
        builder.UseMiddleware<GlobalExceptionHandling>();
    })
    .ConfigureServices(serviceCollection => {
        serviceCollection.AddSingleton<InMemoryDataService>();
    })
    .Build();

host.Run();
