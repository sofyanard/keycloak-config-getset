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
    }
}
