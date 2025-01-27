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

    internal class AuthenticationExecution
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("authenticator")]
        public string? Authenticator { get; set; }

        [JsonPropertyName("authenticationFlow")]
        public bool AuthenticationFlow { get; set; }

        [JsonPropertyName("requirement")]
        public string? Requirement { get; set; }

        [JsonPropertyName("requirementChoices")]
        public List<string>? RequirementChoices { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }

        [JsonPropertyName("userSetupAllowed")]
        public bool UserSetupAllowed { get; set; }

        [JsonPropertyName("autheticatorFlow")]
        public bool AutheticatorFlow { get; set; }

        [JsonPropertyName("flowId")]
        public string? FlowId { get; set; }

        [JsonPropertyName("providerId")]
        public string? ProviderId { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }

    internal class AuthenticationExecutionPost
    {
        [JsonPropertyName("provider")]
        public string? Provider { get; set; }
    }

    internal class Authentication
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("providerId")]
        public string? ProviderId { get; set; }

        [JsonPropertyName("topLevel")]
        public bool TopLevel { get; set; }

        [JsonPropertyName("builtIn")]
        public bool BuiltIn { get; set; }

        [JsonPropertyName("authenticationExecutions")]
        public List<AuthenticationExecution>? AuthenticationExecutions { get; set; }
    }

    internal class AuthenticationPost
    {
        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [JsonPropertyName("providerId")]
        public string? ProviderId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("topLevel")]
        public bool TopLevel { get; set; }

        [JsonPropertyName("builtIn")]
        public bool BuiltIn { get; set; }
    }

    internal class NestedAuthenticationPost
    {
        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("provider")]
        public string? Provider { get; set; }
    }
}
