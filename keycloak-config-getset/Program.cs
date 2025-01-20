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

LoginRequest loginRequest = new LoginRequest
{
    GrantType = "client_credentials",
    Scope = "openid",
    ClientId = srcClientId,
    ClientSecret = srcClientSecret
};

AuthActions.Initialize(logger, configuration);
LoginResponse loginResponse = await AuthActions.LoginAsync("Source");

string? accessToken = loginResponse.AccessToken;
logger.LogInformation("Access Token: {0}", accessToken);

RealmActions.Initialize(logger, configuration);
RealmToken realmToken = await RealmActions.GetRealmTokenAsync("Source", accessToken);

string strRealmToken = JsonSerializer.Serialize(realmToken);
logger.LogInformation("RealmToken: {0}", strRealmToken);

Console.WriteLine("Hello, World!");
