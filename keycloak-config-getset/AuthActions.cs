using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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

        internal static LoginRequest GetLoginRequest(string env)
        {
            return new LoginRequest
            {
                GrantType = "client_credentials",
                Scope = "openid",
                ClientId = _configuration[$"{env}:ClientId"],
                ClientSecret = _configuration[$"{env}:ClientSecret"]
            };
        }

        internal static async Task<LoginResponse> LoginAsync(string env)
        {
            string? host = _configuration["Source:Host"];
            string? realm = _configuration["Source:Realm"];

            var xwfurlencodedLoginRequest = Helpers.ObjectToFormUrlEncoded(GetLoginRequest(env));
            _logger.LogInformation("Request Data: {0}", xwfurlencodedLoginRequest);

            var content = new FormUrlEncodedContent(xwfurlencodedLoginRequest);

            string? loginUrl = $"{host}/realms/{realm}/protocol/openid-connect/token";
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

                    LoginResponse? loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Allows matching JSON keys in a case-insensitive manner
                    }) ?? new LoginResponse();
                    string strLoginResponse = JsonSerializer.Serialize(loginResponse);
                    _logger.LogInformation("LoginResponse : {0}", strLoginResponse);

                    return loginResponse;
                }
                else
                {
                    _logger.LogWarning("Response status: {0}", response.StatusCode);
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Error Details:: {0}", errorContent);

                    throw new Exception($"Login failed: {errorContent}");
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
