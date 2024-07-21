using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Microsoft.Extensions.Azure;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));

        services.AddAzureClients(clientBuilder =>
        {
            var blobServiceClientConnectionString = Environment.GetEnvironmentVariable("PublisherFileStorage");
            clientBuilder.AddBlobServiceClient(blobServiceClientConnectionString);
        });
    })
    .Build();
host.Run();