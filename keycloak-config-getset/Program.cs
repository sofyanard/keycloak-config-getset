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
            // POST Authentication Flow - Level 0
            logger.LogInformation("POST Authentication Flow - Level 0");
            AuthenticationPost authenticationToPost = RealmActions.GetAuthenticationPostFromAuthentication(selectedAuthentication);
            string strAuthenticationToPost = JsonSerializer.Serialize(authenticationToPost);
            logger?.LogInformation("Authentication To Post: {0}", strAuthenticationToPost);

            HttpResponseMessage dstPostResponse = await RealmActions.PostAuthenticationFlowAsync("Destination", dstAccessToken, authenticationToPost);
            string strDstPostResponse = JsonSerializer.Serialize(dstPostResponse);
            logger.LogInformation("HttpResponseMessage: {0}", strDstPostResponse);

            // record list of nested flow in the loop
            List<string> listOfNestedFlow = new List<string>();
            listOfNestedFlow.Clear();
            listOfNestedFlow.Add(selectedAuthentication.Alias!);
            string strListOfNestedFlow = JsonSerializer.Serialize(listOfNestedFlow);
            logger?.LogInformation("List of Nested Flow: {0}", strListOfNestedFlow);

            // GET All Executions for each Flow
            logger.LogInformation("GET All Executions for Flow: {0}", selectedAuthentication.Alias);
            List<AuthenticationExecution> listExecutions = await RealmActions.GetAllExecutionsOfAFlowAsync("Source", srcAccessToken, selectedAuthentication.Alias!);
            string strListExecutions = JsonSerializer.Serialize(listExecutions);
            logger?.LogInformation("List Executions: {0}", strListExecutions);

            foreach (AuthenticationExecution executions in listExecutions)
            {
                string strExecutions = JsonSerializer.Serialize(executions);
                logger?.LogInformation("Processing Executions: {0}", strExecutions);

                int eLevel = executions.Level;
                int eIndex = executions.Index;

                // each item can be either an execution or a nested flow (level 1 and so on), let's first check !
                if (executions.AuthenticationFlow)
                {
                    // POST New Nested Flow
                    logger?.LogInformation("POST New Nested Flow: {0}", executions.DisplayName);
                    if (listOfNestedFlow.Count > eLevel + 1)
                    {
                        listOfNestedFlow[eLevel] = executions.DisplayName!;
                    }
                    else
                    {
                        listOfNestedFlow.Add(executions.DisplayName!);
                    }
                    strListOfNestedFlow = JsonSerializer.Serialize(listOfNestedFlow);
                    logger?.LogInformation("List of Nested Flow: {0}", strListOfNestedFlow);

                    // First, GET Flow By Id
                    Authentication flowToBeCoppied = await RealmActions.GetFlowByIdAsync("Source", srcAccessToken, executions.FlowId!);
                    string strFlowToBeCoppied = JsonSerializer.Serialize(flowToBeCoppied);
                    logger?.LogInformation("Flow To Be Coppied: {0}", strFlowToBeCoppied);
                    
                    // Prepare new nested flow to be POST
                    NestedAuthenticationPost nestedFlowToPost = RealmActions.GetNestedAuthenticationPostFromAuthentication(flowToBeCoppied);
                    string strNestedFlowToPost = JsonSerializer.Serialize(nestedFlowToPost);
                    logger?.LogInformation("Nested Flow To Post: {0}", strNestedFlowToPost);

                    // POST New Nested Flow
                    HttpResponseMessage nfdPostResponse = await RealmActions.PostExecutionsFlowAsync("Destination", dstAccessToken, listOfNestedFlow[eLevel], nestedFlowToPost);
                    string strNfdPostResponse = JsonSerializer.Serialize(nfdPostResponse);
                    logger.LogInformation("HttpResponseMessage: {0}", strNfdPostResponse);
                }
                else
                {
                    // Prepare new execution to be POST
                    logger.LogInformation("POST New Execution");
                    AuthenticationExecutionPost authenticationExecutionPost = RealmActions.GetAuthenticationExecutionPostFromAuthenticationExecution(executions);
                    string strAuthenticationExecutionPost = JsonSerializer.Serialize(authenticationExecutionPost);
                    logger?.LogInformation("Execution To Post: {0}", strAuthenticationExecutionPost);

                    // POST New Execution
                    HttpResponseMessage afePostResponse = await RealmActions.PostExecutionsExecutionAsync("Destination", dstAccessToken, listOfNestedFlow[eLevel], authenticationExecutionPost);
                    string strAfePostResponse = JsonSerializer.Serialize(afePostResponse);
                    logger.LogInformation("HttpResponseMessage: {0}", strAfePostResponse);
                }
            }

            // PUT Executions to update Requirement

            // GET All Executions for each Flow in the Destination Realm
            logger.LogInformation("GET All Executions for Flow: {0} in the Destination Realm", selectedAuthentication.Alias);
            List<AuthenticationExecution> listDestinationExecutions = await RealmActions.GetAllExecutionsOfAFlowAsync("Destination", dstAccessToken, selectedAuthentication.Alias!);
            string strListDestinationExecutions = JsonSerializer.Serialize(listDestinationExecutions);
            logger?.LogInformation("List of Destination Executions: {0}", strListDestinationExecutions);

            foreach (AuthenticationExecution destinationExecution in listDestinationExecutions)
            {
                string strDestinationExecution = JsonSerializer.Serialize(destinationExecution);
                logger?.LogInformation("Destination Execution: {0}", strDestinationExecution);

                // Get each equivalent in the source, for each destination execution
                AuthenticationExecution sourceExecution = listExecutions.FirstOrDefault(x => x.DisplayName == destinationExecution.DisplayName)!;

                if (sourceExecution == null)
                {
                    logger?.LogError("Equivalent Source Execution not found!");
                    throw new Exception("Source Execution not found!");
                }

                string strSourceExecution = JsonSerializer.Serialize(sourceExecution);
                logger?.LogInformation("Equivalent Source Execution: {0}", strSourceExecution);

                // Prepare execution to be PUT
                AuthenticationExecutionPut authenticationExecutionPut = RealmActions.GetUpdatedAuthenticationExecutionPutFromSourceToDestination(destinationExecution, sourceExecution);
                string strAuthenticationExecutionPut = JsonSerializer.Serialize(authenticationExecutionPut);
                logger?.LogInformation("Execution to PUT: {0}", strAuthenticationExecutionPut);

                // PUT The Execution
                HttpResponseMessage putExecutionResponse = await RealmActions.PutExecutionsExecutionAsync("Destination", dstAccessToken, selectedAuthentication.Alias!, authenticationExecutionPut);
                string strPutExecutionResponse = JsonSerializer.Serialize(putExecutionResponse);
                logger.LogInformation("HttpResponseMessage: {0}", strPutExecutionResponse);
            }

            j++;
        }
        AppModels.PauseMessage($"{j} Authentication Flow(s) have been posted!");

        AppModels.ShowMenu(AppModels.GetMainMenu());
        input = Console.ReadLine();
    }
    else if (input.Equals("3"))
    {
        // Get All Clients
        List<Client> listClient = await RealmActions.GetAllClientAsync("Source", srcAccessToken!);
        string strListClient = JsonSerializer.Serialize(listClient);
        logger.LogInformation("List Clients: {0}", strListClient);

        AppModels.PauseMessage("Get All Clients...");

        // Client Custom Menu
        List<CustomMenu> listClientMenu = AppModels.ConvertToCustomMenu(listClient, "ClientId");
        foreach (CustomMenu clientMenu in listClientMenu)
        {
            Console.WriteLine($"{clientMenu.Id} - {clientMenu.Name}");
        }

        // Selection of Authentication Flow to process
        Console.WriteLine("Choose Sequence Number of Clients to be copied, separate with coma(,)!");
        string? inputClient = Console.ReadLine();
        List<CustomMenu> listSelectedClientMenu = new List<CustomMenu>();
        List<Client> listSelectedClient = new List<Client>();

        try
        {
            int i;
            string[] choosenInputClient = inputClient.Split(",");
            foreach (string choice in choosenInputClient)
            {
                choice.Trim();
                i = int.Parse(choice);
                CustomMenu? selectedClientMenu = listClientMenu.FirstOrDefault(x => x.Id == i);
                Client? toBeAddClient = listClient.FirstOrDefault(x => x.ClientId == selectedClientMenu?.Name);
                if ((selectedClientMenu == null) || (toBeAddClient == null))
                {
                    throw new Exception("Wrong number to choose!");
                }
                string strSelectedClientMenu = JsonSerializer.Serialize(selectedClientMenu);
                logger?.LogInformation("Selected Client Menu: {0}", strSelectedClientMenu);

                listSelectedClientMenu.Add(selectedClientMenu);
                listSelectedClient.Add(toBeAddClient);
            }
        }
        catch (Exception e)
        {
            logger?.LogError(e, e.Message);
            throw;
        }

        string strListSelectedClient = JsonSerializer.Serialize(listSelectedClient);
        logger?.LogInformation("List Selected Client: {0}", strListSelectedClient);

        AppModels.ShowCustomMenu(listSelectedClientMenu, "Selected Client");
        AppModels.PauseMessage("Are You sure to set these Client?");

        // POST Client to Destination Realm
        int k = 0;
        foreach (Client selectedClient in listSelectedClient)
        {
            // POST Client
            logger.LogInformation("POST Client");
            ClientPost clientToPost = RealmActions.GetClientPostFromClient(selectedClient);
            string strClientToPost = JsonSerializer.Serialize(clientToPost);
            logger?.LogInformation("Client To Post: {0}", strClientToPost);

            HttpResponseMessage clientPostResponse = await RealmActions.PostClientAsync("Destination", dstAccessToken, clientToPost);
            string strClientPostResponse = JsonSerializer.Serialize(clientPostResponse);
            logger.LogInformation("HttpResponseMessage: {0}", strClientPostResponse);

            k++;
        }
        
        AppModels.PauseMessage($"{k} Client(s) have been posted!");



        AppModels.ShowMenu(AppModels.GetMainMenu());
        input = Console.ReadLine();
    }
    else
    {
        exit = true;
    }
}



Console.WriteLine("Hello, World!");
