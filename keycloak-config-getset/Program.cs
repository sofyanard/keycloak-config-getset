using keycloak_config_getset;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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

logger.LogInformation("Source Realm: {0} - {1} - {2} - {3}", srcHost, srcRealm, srcClientId, srcClientSecret);
logger.LogInformation("Destination Realm: {0} - {1} - {2} - {3}", dstHost, dstRealm, dstClientId, dstClientSecret);

AppModels.PauseMessage("Loading Configuration...");

// Initialize Actions Classes
AuthActions.Initialize(logger, configuration);
RealmActions.Initialize(logger, configuration);

AppModels.PauseMessage("Initializing...");



// Login to Source Realm
LoginResponse srcLoginResponse = await AuthActions.LoginAsync("Source");
string srcAccessToken = srcLoginResponse.AccessToken;
logger.LogInformation("Source Access Token: {0}", srcAccessToken);

AppModels.PauseMessage("Login to Source Realm...");



// Login to Destination Realm
LoginResponse dstLoginResponse = await AuthActions.LoginAsync("Destination");
string dstAccessToken = dstLoginResponse.AccessToken;
logger.LogInformation("Destination Access Token: {0}", srcAccessToken);

AppModels.PauseMessage("Login to Destination Realm...");



///////////////
// Main Menu //
///////////////
bool exit = false;

AppModels.ShowMenu(AppModels.GetMainMenu());
string? input = Console.ReadLine();

while (!exit)
{
    if (input.Equals("1"))
    {
        // Get Source Realm Token Settings
        RealmToken srcRealmToken = await RealmActions.GetRealmTokenAsync("Source", srcAccessToken);
        string strSrcRealmToken = JsonSerializer.Serialize(srcRealmToken);
        logger.LogInformation("Source RealmToken: {0}", strSrcRealmToken);

        AppModels.PauseMessage("Get Source Realm Token Settings...");

        // Put Token Settings to Destination Realm
        logger.LogInformation("Put Token Settings to Destination Realm");
        HttpResponseMessage dstPutResponse = await RealmActions.PutRealmTokenAsync("Destination", dstAccessToken, srcRealmToken);
        string srcDstPutResponse = JsonSerializer.Serialize(dstPutResponse);
        logger.LogInformation("HttpResponseMessage: {0}", srcDstPutResponse);

        AppModels.PauseMessage("Put Token Settings to Destination Realm...");

        AppModels.ShowMenu(AppModels.GetMainMenu());
        input = Console.ReadLine();
    }
    else if (input.Equals("2"))
    {
        // Get All Authentication Flows
        List<Authentication> listAuthentication = await RealmActions.GetAllAuthenticationFlowAsync("Source", srcAccessToken);
        string strListAuthentication = JsonSerializer.Serialize(listAuthentication);
        logger.LogInformation("List Authentication: {0}", strListAuthentication);

        AppModels.PauseMessage("Get All Authentication Flows...");

        // Authentication Flow Custom Menu
        List<CustomMenu> listAuthenticationFlowMenu = AppModels.ConvertToCustomMenu(listAuthentication, "Alias");
        foreach (CustomMenu authenticationFlowMenu in listAuthenticationFlowMenu)
        {
            Console.WriteLine($"{authenticationFlowMenu.Id} - {authenticationFlowMenu.Name}");
        }

        // Selection of Authentication Flow to process
        Console.WriteLine("Choose Sequence Number Authentication Flow to be copied, separate with coma(,)!");
        string? inputFlow = Console.ReadLine();
        List<CustomMenu> listSelectedAuthenticationMenu = new List<CustomMenu>();
        List<Authentication> listSelectedAuthentication = new List<Authentication>();
        try
        {
            int i;
            string[] choosenInputFlow = inputFlow.Split(",");
            foreach (string choice in choosenInputFlow)
            {
                choice.Trim();
                i = int.Parse(choice);
                CustomMenu? selectedAuthenticationMenu = listAuthenticationFlowMenu.FirstOrDefault(x => x.Id == i);
                Authentication? toBeAddAuthentication = listAuthentication.FirstOrDefault(x => x.Alias == selectedAuthenticationMenu?.Name);
                if ((selectedAuthenticationMenu == null) || (toBeAddAuthentication == null))
                {
                    throw new Exception("Wrong number to choose!");
                }
                string strSelectedAuthenticationMenu = JsonSerializer.Serialize(selectedAuthenticationMenu);
                logger?.LogInformation("Selected Authentication Menu: {0}", strSelectedAuthenticationMenu);

                listSelectedAuthenticationMenu.Add(selectedAuthenticationMenu);
                listSelectedAuthentication.Add(toBeAddAuthentication);
            }
        }
        catch (Exception e)
        {
            logger?.LogError(e, e.Message);
            throw;
        }

        string strListSelectedAuthentication = JsonSerializer.Serialize(listSelectedAuthentication);
        logger?.LogInformation("List Selected Authentication: {0}", strListSelectedAuthentication);

        AppModels.ShowCustomMenu(listSelectedAuthenticationMenu, "Selected Authentication Flow");
        AppModels.PauseMessage("Are You sure to set these Authentication Flow?");

        // POST Authentication Flow to Destination Realm
        int j = 0;
        foreach (Authentication selectedAuthentication in listSelectedAuthentication)
        {
            logger.LogInformation("POST Authentication Flow to Destination Realm");
            AuthenticationPost authenticationToPost = RealmActions.GetAuthenticationPostFromAuthentication(selectedAuthentication);
            string strAuthenticationToPost = JsonSerializer.Serialize(authenticationToPost);
            logger?.LogInformation("Authentication To Post: {0}", strAuthenticationToPost);

            HttpResponseMessage dstPostResponse = await RealmActions.PostAuthenticationFlowAsync("Destination", dstAccessToken, authenticationToPost);
            string strDstPostResponse = JsonSerializer.Serialize(dstPostResponse);
            logger.LogInformation("HttpResponseMessage: {0}", strDstPostResponse);

            j++;
        }
        AppModels.PauseMessage($"{j} Authentication Flow(s) have been posted!");

        AppModels.ShowMenu(AppModels.GetMainMenu());
        input = Console.ReadLine();
    }
    else
    {
        exit = true;
    }
}



Console.WriteLine("Hello, World!");
