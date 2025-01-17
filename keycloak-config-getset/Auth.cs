using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace keycloak_config_getset
{
    internal static class AuthActions
    {
        private static ILogger _logger;
        private static IConfiguration _configuration;

        internal static void Initialize(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _logger.LogInformation("Auth is Initialized");
        }

        internal static async Task LoginAsync(LoginRequest loginRequest)
        {
            string? srcHost = _configuration["Source:Host"];
            string? srcRealm = _configuration["Source:Realm"];
            string? srcClientId = _configuration["Source:ClientId"];
            string? srcClientSecret = _configuration["Source:ClientSecret"];

            var xwfurlencodedLoginRequest = Helpers.ObjectToFormUrlEncoded(loginRequest);
            _logger.LogInformation("Request Data: {0}", xwfurlencodedLoginRequest);

            var content = new FormUrlEncodedContent(xwfurlencodedLoginRequest);

            string? loginUrl = $"{srcHost}/realms/{srcRealm}/protocol/openid-connect/token";
            _logger.LogInformation("Login Url: {0}", loginUrl);

            // Bypass SSL certificate validation
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };

            using var httpClient = new HttpClient(handler);

            try
            {
                _logger.LogInformation("Starting request...");
                var response = await httpClient.PostAsync(loginUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Response status is success...");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Response content : {0}", responseContent);
                }
                else
                {
                    _logger.LogInformation("Response status: {0}", response.StatusCode);
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error Details:");
                    Console.WriteLine(errorContent);
                    _logger.LogInformation("Error Details:: {0}", errorContent);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
