using GeoTabTest.Application;
using GeoTabTest.Application.Interfaces;
using GeoTabTest.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {

        IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
        var options = configuration.GetSection("GeotabApiAuthenticationConfig").Get<GeotabApiAuthenticationConfig>();
        services.AddSingleton<GeotabApiAuthenticationConfig>(options);
        services.AddSingleton<IAPIProviderService, APIProviderService>();
        services.AddTransient<IDevicesService, DevicesService>();
        services.AddHostedService<Worker>();
    })
    .Build();
host.Run();
