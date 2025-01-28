using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace keycloak_config_getset
{
    internal static class Helpers
    {
        internal static IEnumerable<KeyValuePair<string, string>> ObjectToFormUrlEncoded(object obj)
        {
            var dict = new Dictionary<string, string>();
            foreach (var prop in obj.GetType().GetProperties())
            {
                // Check if JsonPropertyName attribute is applied
                var jsonPropertyNameAttribute = prop.GetCustomAttribute<JsonPropertyNameAttribute>();

                // Use JsonPropertyName value if it exists, otherwise default to the property name
                var key = jsonPropertyNameAttribute?.Name ?? prop.Name;

                // Get the value of the property
                var value = prop.GetValue(obj)?.ToString();
                if (value != null)
                {
                    dict.Add(key, value);
                }
            }
            return dict;
        }

        internal static string GetClientUuidFromResponse(string response)
        {
            var responseObj = System.Text.Json.JsonSerializer.Deserialize<HttpResponseMessageWrapper>(response);
            var locationHeader = responseObj.Headers?.FirstOrDefault(h => h.Key == "Location");
            if (locationHeader == null)
            {
                throw new Exception("Location header not found in response");
            }
            var location = locationHeader.Value?.FirstOrDefault();
            if (location == null)
            {
                throw new Exception("Location header value not found in response");
            }
            var parts = location.Split('/');
            return parts.Last();
        }
    }

    public class HttpResponseMessageWrapper
    {
        [JsonPropertyName("Version")]
        public string? Version { get; set; }

        [JsonPropertyName("Content")]
        public ContentWrapper? Content { get; set; }

        [JsonPropertyName("StatusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("ReasonPhrase")]
        public string? ReasonPhrase { get; set; }

        [JsonPropertyName("Headers")]
        public List<HeaderWrapper>? Headers { get; set; }

        [JsonPropertyName("TrailingHeaders")]
        public List<HeaderWrapper>? TrailingHeaders { get; set; }

        [JsonPropertyName("RequestMessage")]
        public RequestMessageWrapper? RequestMessage { get; set; }

        [JsonPropertyName("IsSuccessStatusCode")]
        public bool IsSuccessStatusCode { get; set; }
    }

    public class ContentWrapper
    {
        [JsonPropertyName("Headers")]
        public List<HeaderWrapper>? Headers { get; set; }
    }

    public class HeaderWrapper
    {
        [JsonPropertyName("Key")]
        public string? Key { get; set; }

        [JsonPropertyName("Value")]
        public List<string>? Value { get; set; }
    }

    public class RequestMessageWrapper
    {
        [JsonPropertyName("Version")]
        public string? Version { get; set; }

        [JsonPropertyName("VersionPolicy")]
        public int VersionPolicy { get; set; }

        [JsonPropertyName("Content")]
        public ContentWrapper? Content { get; set; }

        [JsonPropertyName("Method")]
        public MethodWrapper? Method { get; set; }

        [JsonPropertyName("RequestUri")]
        public string? RequestUri { get; set; }

        [JsonPropertyName("Headers")]
        public List<HeaderWrapper>? Headers { get; set; }

        [JsonPropertyName("Properties")]
        public Dictionary<string, object>? Properties { get; set; }

        [JsonPropertyName("Options")]
        public Dictionary<string, object>? Options { get; set; }
    }

    public class MethodWrapper
    {
        [JsonPropertyName("Method")]
        public string? Method { get; set; }
    }
}
