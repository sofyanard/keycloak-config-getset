using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace keycloak_config_getset
{
    internal class RealmAttribute
    {
        [JsonPropertyName("cibaBackchannelTokenDeliveryMode")]
        public string? CibaBackchannelTokenDeliveryMode { get; set; }

        [JsonPropertyName("cibaExpiresIn")]
        public string? CibaExpiresIn { get; set; }

        [JsonPropertyName("cibaAuthRequestedUserHint")]
        public string? CibaAuthRequestedUserHint { get; set; }

        [Description("OAuth 2.0 Device Code Lifespan")]
        [JsonPropertyName("oauth2DeviceCodeLifespan")]
        public string? Oauth2DeviceCodeLifespan { get; set; }

        [Description("OAuth 2.0 Device Polling Interval")]
        [JsonPropertyName("oauth2DevicePollingInterval")]
        public string? Oauth2DevicePollingInterval { get; set; }

        [JsonPropertyName("clientOfflineSessionMaxLifespan")]
        public string? ClientOfflineSessionMaxLifespan { get; set; }

        [JsonPropertyName("clientSessionIdleTimeout")]
        public string? ClientSessionIdleTimeout { get; set; }

        [JsonPropertyName("userProfileEnabled")]
        public string? UserProfileEnabled { get; set; }

        [Description("Lifetime of the Request URI for Pushed Authorization Request")]
        [JsonPropertyName("parRequestUriLifespan")]
        public string? ParRequestUriLifespan { get; set; }

        [JsonPropertyName("clientSessionMaxLifespan")]
        public string? ClientSessionMaxLifespan { get; set; }

        [JsonPropertyName("clientOfflineSessionIdleTimeout")]
        public string? ClientOfflineSessionIdleTimeout { get; set; }

        [JsonPropertyName("cibaInterval")]
        public string? CibaInterval { get; set; }
    }

    internal class RealmToken
    {
        [Description("Default Signature Algorithm")]
        [JsonPropertyName("defaultSignatureAlgorithm")]
        public string? DefaultSignatureAlgorithm { get; set; }

        [Description("Revoke Refresh Token")]
        [JsonPropertyName("revokeRefreshToken")]
        public bool RevokeRefreshToken { get; set; }

        [Description("SSO Session Idle")]
        [JsonPropertyName("ssoSessionIdleTimeout")]
        public int SsoSessionIdleTimeout { get; set; }

        [Description("SSO Session Max")]
        [JsonPropertyName("ssoSessionMaxLifespan")]
        public int SsoSessionMaxLifespan { get; set; }

        [Description("SSO Session Idle Remember Me")]
        [JsonPropertyName("ssoSessionIdleTimeoutRememberMe")]
        public int SsoSessionIdleTimeoutRememberMe { get; set; }

        [Description("SSO Session Max Remember Me")]
        [JsonPropertyName("ssoSessionMaxLifespanRememberMe")]
        public int SsoSessionMaxLifespanRememberMe { get; set; }

        [Description("Offline Session Idle")]
        [JsonPropertyName("offlineSessionIdleTimeout")]
        public int OfflineSessionIdleTimeout { get; set; }

        [Description("Offline Session Max Limited")]
        [JsonPropertyName("offlineSessionMaxLifespanEnabled")]
        public bool OfflineSessionMaxLifespanEnabled { get; set; }

        [Description("Client Session Idle")]
        [JsonPropertyName("clientSessionIdleTimeout")]
        public int ClientSessionIdleTimeout { get; set; }

        [Description("Client Session Max")]
        [JsonPropertyName("clientSessionMaxLifespan")]
        public int ClientSessionMaxLifespan { get; set; }

        [Description("Access Token Lifespan")]
        [JsonPropertyName("accessTokenLifespan")]
        public int AccessTokenLifespan { get; set; }

        [Description("Access Token Lifespan For Implicit Flow")]
        [JsonPropertyName("accessTokenLifespanForImplicitFlow")]
        public int AccessTokenLifespanForImplicitFlow { get; set; }

        [Description("Client login timeout")]
        [JsonPropertyName("accessCodeLifespan")]
        public int AccessCodeLifespan { get; set; }

        [Description("Login timeout")]
        [JsonPropertyName("accessCodeLifespanLogin")]
        public int AccessCodeLifespanLogin { get; set; }

        [Description("Login action timeout")]
        [JsonPropertyName("accessCodeLifespanUserAction")]
        public int AccessCodeLifespanUserAction { get; set; }

        [Description("User-Initiated Action Lifespan")]
        [JsonPropertyName("actionTokenGeneratedByUserLifespan")]
        public int ActionTokenGeneratedByUserLifespan { get; set; }

        [Description("Default Admin-Initiated Action Lifespan")]
        [JsonPropertyName("actionTokenGeneratedByAdminLifespan")]
        public int ActionTokenGeneratedByAdminLifespan { get; set; }

        [Description("OAuth 2.0 Device Code Lifespan")]
        [JsonPropertyName("oauth2DeviceCodeLifespan")]
        public int Oauth2DeviceCodeLifespan { get; set; }

        [JsonPropertyName("attributes")]
        public RealmAttribute? TokenAttribute { get; set; }
    }
}
