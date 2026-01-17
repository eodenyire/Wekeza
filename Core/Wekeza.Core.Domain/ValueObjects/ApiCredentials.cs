using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.ValueObjects;

/// <summary>
/// API Credentials Value Object - Represents authentication credentials for external APIs
/// Immutable value object for secure credential management
/// Industry Standard: OAuth 2.0, JWT, API Key authentication patterns
/// </summary>
public class ApiCredentials : ValueObject
{
    public string ApiKey { get; }
    public string ClientId { get; }
    public string ClientSecret { get; }
    public string AccessToken { get; }
    public string RefreshToken { get; }
    public DateTime? ExpiresAt { get; }
    public string TokenType { get; }
    public string Scope { get; }
    public Dictionary<string, string> AdditionalProperties { get; }
    public AuthenticationType AuthType { get; }

    public ApiCredentials(
        AuthenticationType authType,
        string apiKey = null,
        string clientId = null,
        string clientSecret = null,
        string accessToken = null,
        string refreshToken = null,
        DateTime? expiresAt = null,
        string tokenType = "Bearer",
        string scope = null,
        Dictionary<string, string> additionalProperties = null)
    {
        // Validation based on authentication type
        ValidateCredentials(authType, apiKey, clientId, clientSecret, accessToken);

        AuthType = authType;
        ApiKey = apiKey;
        ClientId = clientId;
        ClientSecret = clientSecret;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
        TokenType = tokenType ?? "Bearer";
        Scope = scope;
        AdditionalProperties = additionalProperties ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Check if the access token is expired
    /// </summary>
    public bool IsExpired(DateTime currentTime)
    {
        return ExpiresAt.HasValue && currentTime >= ExpiresAt.Value;
    }

    /// <summary>
    /// Check if credentials are valid based on authentication type
    /// </summary>
    public bool IsValid()
    {
        return AuthType switch
        {
            AuthenticationType.None => true,
            AuthenticationType.ApiKey => !string.IsNullOrWhiteSpace(ApiKey),
            AuthenticationType.BasicAuth => !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret),
            AuthenticationType.BearerToken => !string.IsNullOrWhiteSpace(AccessToken),
            AuthenticationType.OAuth2 => !string.IsNullOrWhiteSpace(AccessToken) && !string.IsNullOrWhiteSpace(ClientId),
            AuthenticationType.JWT => !string.IsNullOrWhiteSpace(AccessToken),
            AuthenticationType.Certificate => AdditionalProperties.ContainsKey("CertificateThumbprint"),
            AuthenticationType.HMAC => !string.IsNullOrWhiteSpace(ClientSecret),
            AuthenticationType.Custom => AdditionalProperties.Any(),
            _ => false
        };
    }

    /// <summary>
    /// Get the authorization header value for HTTP requests
    /// </summary>
    public string GetAuthorizationHeader()
    {
        return AuthType switch
        {
            AuthenticationType.ApiKey => ApiKey,
            AuthenticationType.BasicAuth => $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"))}",
            AuthenticationType.BearerToken => $"{TokenType} {AccessToken}",
            AuthenticationType.OAuth2 => $"{TokenType} {AccessToken}",
            AuthenticationType.JWT => $"Bearer {AccessToken}",
            AuthenticationType.Custom => AdditionalProperties.GetValueOrDefault("AuthorizationHeader", ""),
            _ => string.Empty
        };
    }

    /// <summary>
    /// Get API key for query parameter or header
    /// </summary>
    public string GetApiKey()
    {
        return ApiKey ?? AdditionalProperties.GetValueOrDefault("ApiKey", "");
    }

    /// <summary>
    /// Get client credentials for OAuth 2.0 flows
    /// </summary>
    public (string ClientId, string ClientSecret) GetClientCredentials()
    {
        return (ClientId, ClientSecret);
    }

    /// <summary>
    /// Get access token with type
    /// </summary>
    public (string Token, string Type) GetAccessToken()
    {
        return (AccessToken, TokenType);
    }

    /// <summary>
    /// Check if token needs refresh (expires within specified minutes)
    /// </summary>
    public bool NeedsRefresh(DateTime currentTime, int bufferMinutes = 5)
    {
        if (!ExpiresAt.HasValue)
            return false;

        return currentTime.AddMinutes(bufferMinutes) >= ExpiresAt.Value;
    }

    /// <summary>
    /// Create new credentials with refreshed token
    /// </summary>
    public ApiCredentials RefreshCredentials(string newAccessToken, DateTime newExpiresAt, string newRefreshToken = null)
    {
        if (string.IsNullOrWhiteSpace(newAccessToken))
            throw new ArgumentException("New access token cannot be empty", nameof(newAccessToken));

        if (newExpiresAt <= DateTime.UtcNow)
            throw new ArgumentException("New expiration must be in the future", nameof(newExpiresAt));

        return new ApiCredentials(
            AuthType,
            ApiKey,
            ClientId,
            ClientSecret,
            newAccessToken,
            newRefreshToken ?? RefreshToken,
            newExpiresAt,
            TokenType,
            Scope,
            AdditionalProperties);
    }

    /// <summary>
    /// Create new credentials with updated API key
    /// </summary>
    public ApiCredentials UpdateApiKey(string newApiKey)
    {
        if (string.IsNullOrWhiteSpace(newApiKey))
            throw new ArgumentException("New API key cannot be empty", nameof(newApiKey));

        return new ApiCredentials(
            AuthType,
            newApiKey,
            ClientId,
            ClientSecret,
            AccessToken,
            RefreshToken,
            ExpiresAt,
            TokenType,
            Scope,
            AdditionalProperties);
    }

    /// <summary>
    /// Create new credentials with updated client credentials
    /// </summary>
    public ApiCredentials UpdateClientCredentials(string newClientId, string newClientSecret)
    {
        if (string.IsNullOrWhiteSpace(newClientId))
            throw new ArgumentException("New client ID cannot be empty", nameof(newClientId));

        if (string.IsNullOrWhiteSpace(newClientSecret))
            throw new ArgumentException("New client secret cannot be empty", nameof(newClientSecret));

        return new ApiCredentials(
            AuthType,
            ApiKey,
            newClientId,
            newClientSecret,
            AccessToken,
            RefreshToken,
            ExpiresAt,
            TokenType,
            Scope,
            AdditionalProperties);
    }

    /// <summary>
    /// Add or update additional property
    /// </summary>
    public ApiCredentials WithAdditionalProperty(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Property key cannot be empty", nameof(key));

        var newProperties = new Dictionary<string, string>(AdditionalProperties)
        {
            [key] = value ?? string.Empty
        };

        return new ApiCredentials(
            AuthType,
            ApiKey,
            ClientId,
            ClientSecret,
            AccessToken,
            RefreshToken,
            ExpiresAt,
            TokenType,
            Scope,
            newProperties);
    }

    /// <summary>
    /// Get additional property value
    /// </summary>
    public string GetAdditionalProperty(string key, string defaultValue = null)
    {
        return AdditionalProperties.GetValueOrDefault(key, defaultValue);
    }

    /// <summary>
    /// Check if credentials have specific capability
    /// </summary>
    public bool HasCapability(string capability)
    {
        if (string.IsNullOrWhiteSpace(capability))
            return false;

        // Check scope for OAuth 2.0
        if (!string.IsNullOrWhiteSpace(Scope))
        {
            var scopes = Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (scopes.Contains(capability, StringComparer.OrdinalIgnoreCase))
                return true;
        }

        // Check additional properties
        return AdditionalProperties.ContainsKey($"capability.{capability}") ||
               AdditionalProperties.ContainsKey($"scope.{capability}");
    }

    /// <summary>
    /// Get time until token expiration
    /// </summary>
    public TimeSpan? GetTimeUntilExpiration(DateTime currentTime)
    {
        if (!ExpiresAt.HasValue)
            return null;

        var timeUntilExpiration = ExpiresAt.Value - currentTime;
        return timeUntilExpiration > TimeSpan.Zero ? timeUntilExpiration : TimeSpan.Zero;
    }

    /// <summary>
    /// Create credentials for API Key authentication
    /// </summary>
    public static ApiCredentials ForApiKey(string apiKey)
    {
        return new ApiCredentials(AuthenticationType.ApiKey, apiKey: apiKey);
    }

    /// <summary>
    /// Create credentials for Basic Authentication
    /// </summary>
    public static ApiCredentials ForBasicAuth(string username, string password)
    {
        return new ApiCredentials(AuthenticationType.BasicAuth, clientId: username, clientSecret: password);
    }

    /// <summary>
    /// Create credentials for Bearer Token authentication
    /// </summary>
    public static ApiCredentials ForBearerToken(string token, DateTime? expiresAt = null)
    {
        return new ApiCredentials(AuthenticationType.BearerToken, accessToken: token, expiresAt: expiresAt);
    }

    /// <summary>
    /// Create credentials for OAuth 2.0
    /// </summary>
    public static ApiCredentials ForOAuth2(string clientId, string clientSecret, string accessToken, 
        string refreshToken = null, DateTime? expiresAt = null, string scope = null)
    {
        return new ApiCredentials(AuthenticationType.OAuth2, clientId: clientId, clientSecret: clientSecret,
            accessToken: accessToken, refreshToken: refreshToken, expiresAt: expiresAt, scope: scope);
    }

    /// <summary>
    /// Create credentials for JWT authentication
    /// </summary>
    public static ApiCredentials ForJWT(string jwtToken, DateTime? expiresAt = null)
    {
        return new ApiCredentials(AuthenticationType.JWT, accessToken: jwtToken, expiresAt: expiresAt);
    }

    /// <summary>
    /// Create credentials for HMAC authentication
    /// </summary>
    public static ApiCredentials ForHMAC(string secretKey, Dictionary<string, string> additionalProperties = null)
    {
        return new ApiCredentials(AuthenticationType.HMAC, clientSecret: secretKey, 
            additionalProperties: additionalProperties);
    }

    /// <summary>
    /// Create empty credentials (no authentication)
    /// </summary>
    public static ApiCredentials None()
    {
        return new ApiCredentials(AuthenticationType.None);
    }

    /// <summary>
    /// Validate credentials based on authentication type
    /// </summary>
    private static void ValidateCredentials(AuthenticationType authType, string apiKey, string clientId, 
        string clientSecret, string accessToken)
    {
        switch (authType)
        {
            case AuthenticationType.ApiKey:
                if (string.IsNullOrWhiteSpace(apiKey))
                    throw new ArgumentException("API key is required for API key authentication");
                break;
            
            case AuthenticationType.BasicAuth:
                if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                    throw new ArgumentException("Client ID and secret are required for basic authentication");
                break;
            
            case AuthenticationType.BearerToken:
            case AuthenticationType.JWT:
                if (string.IsNullOrWhiteSpace(accessToken))
                    throw new ArgumentException("Access token is required for bearer/JWT authentication");
                break;
            
            case AuthenticationType.OAuth2:
                if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(accessToken))
                    throw new ArgumentException("Client ID and access token are required for OAuth 2.0");
                break;
            
            case AuthenticationType.HMAC:
                if (string.IsNullOrWhiteSpace(clientSecret))
                    throw new ArgumentException("Secret key is required for HMAC authentication");
                break;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AuthType;
        yield return ApiKey ?? string.Empty;
        yield return ClientId ?? string.Empty;
        yield return ClientSecret ?? string.Empty;
        yield return AccessToken ?? string.Empty;
        yield return RefreshToken ?? string.Empty;
        yield return ExpiresAt;
        yield return TokenType ?? string.Empty;
        yield return Scope ?? string.Empty;
        
        // Include additional properties in equality comparison
        foreach (var kvp in AdditionalProperties.OrderBy(x => x.Key))
        {
            yield return kvp.Key;
            yield return kvp.Value;
        }
    }

    public override string ToString()
    {
        var maskedInfo = AuthType switch
        {
            AuthenticationType.ApiKey => $"ApiKey: {MaskSecret(ApiKey)}",
            AuthenticationType.BasicAuth => $"BasicAuth: {ClientId}:{MaskSecret(ClientSecret)}",
            AuthenticationType.BearerToken => $"Bearer: {MaskSecret(AccessToken)}",
            AuthenticationType.OAuth2 => $"OAuth2: {ClientId}, Token: {MaskSecret(AccessToken)}",
            AuthenticationType.JWT => $"JWT: {MaskSecret(AccessToken)}",
            AuthenticationType.HMAC => $"HMAC: {MaskSecret(ClientSecret)}",
            AuthenticationType.None => "None",
            _ => "Custom"
        };

        var expirationInfo = ExpiresAt.HasValue ? $", Expires: {ExpiresAt:yyyy-MM-dd HH:mm:ss}" : "";
        return $"{AuthType} ({maskedInfo}{expirationInfo})";
    }

    /// <summary>
    /// Mask sensitive information for logging/display
    /// </summary>
    private static string MaskSecret(string secret)
    {
        if (string.IsNullOrWhiteSpace(secret))
            return "***";

        if (secret.Length <= 8)
            return "***";

        return $"{secret[..4]}***{secret[^4..]}";
    }
}