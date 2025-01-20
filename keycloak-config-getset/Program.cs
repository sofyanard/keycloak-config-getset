using keycloak_config_getset;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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

string? srcHost = configuration["Source:Host"];
string? srcRealm = configuration["Source:Realm"];
string? srcClientId = configuration["Source:ClientId"];
string? srcClientSecret = configuration["Source:ClientSecret"];

string? dstHost = configuration["Destination:Host"];
string? dstRealm = configuration["Destination:Realm"];
string? dstClientId = configuration["Destination:ClientId"];
string? dstClientSecret = configuration["Destination:ClientSecret"];

logger.LogInformation("{0} - {1} - {2} - {3}", srcHost, srcRealm, srcClientId, srcClientSecret);
logger.LogInformation("{0} - {1} - {2} - {3}", dstHost, dstRealm, dstClientId, dstClientSecret);



// Initialize Actions Classes
AuthActions.Initialize(logger, configuration);
RealmActions.Initialize(logger, configuration);



// Login to Source Realm
LoginResponse srcLoginResponse = await AuthActions.LoginAsync("Source");
string srcAccessToken = srcLoginResponse.AccessToken;
logger.LogInformation("Source Access Token: {0}", srcAccessToken);



// Get Source Realm Token Settings
RealmToken srcRealmToken = await RealmActions.GetRealmTokenAsync("Source", srcAccessToken);
string strSrcRealmToken = JsonSerializer.Serialize(srcRealmToken);
logger.LogInformation("RealmToken: {0}", strSrcRealmToken);



Console.WriteLine("Hello, World!");
