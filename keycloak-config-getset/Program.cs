// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Information);
});
var logger = loggerFactory.CreateLogger<Program>();

var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("keycloak-config-getset/appsettings.json", optional: false, reloadOnChange: true)
        .Build();

var srcHost = configuration["Source:Host"];
var srcRealm = configuration["Source:Realm"];
var srcClientId = configuration["Source:ClientId"];
var srcClientSecret = configuration["Source:ClientSecret"];

var dstHost = configuration["Destination:Host"];
var dstRealm = configuration["Destination:Realm"];
var dstClientId = configuration["Destination:ClientId"];
var dstClientSecret = configuration["Destination:ClientSecret"];

logger.LogInformation("{0} - {1} - {2} - {3}", srcHost, srcRealm, srcClientId, srcClientSecret);
logger.LogInformation("{0} - {1} - {2} - {3}", dstHost, dstRealm, dstClientId, dstClientSecret);

Console.WriteLine("Hello, World!");
