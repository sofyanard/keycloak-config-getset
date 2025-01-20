using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System;
using System.Text.Json;
using System.Xml.Linq;

namespace keycloak_config_getset
{
    internal class RealmActions
    {
        private static ILogger? _logger;
        private static IConfiguration? _configuration;

        internal static void Initialize(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _logger.LogInformation("RealmActions is Initialized");
        }

        internal static async Task<RealmToken> GetRealmTokenAsync(string env, string accessToken)
        {
            string? host = _configuration?[$"{env}:Host"];
            string? realm = _configuration?[$"{env}:Realm"];

            string? realmTokenUrl = $"{host}/admin/realms/{realm}";
            _logger?.LogInformation("Realm Token Url: {0}", realmTokenUrl);

            // Bypass SSL certificate validation
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };

            using var httpClient = new HttpClient(handler);

            try
            {
                _logger?.LogInformation("Starting request...");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var request = new HttpRequestMessage(HttpMethod.Get, realmTokenUrl);
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                _logger?.LogInformation("Request status is success");
                string jsonString = await response.Content.ReadAsStringAsync();
                _logger?.LogInformation("Response Content: {0}", jsonString);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                RealmToken? realmToken = JsonSerializer.Deserialize<RealmToken>(jsonString, options);
                
                string strRealmToken = JsonSerializer.Serialize(realmToken);
                _logger?.LogInformation("RealmToken: {0}", strRealmToken);

                return realmToken;
            }
            catch (Exception e)
            {
                _logger?.LogError(e, e.Message);
                throw;
            }
        }
    }
}
