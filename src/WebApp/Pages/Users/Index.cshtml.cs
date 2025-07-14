using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace WebApp.Pages.Users;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public AuthenticatedUserDto User { get; private set; } = new();

    public void OnGet()
    {
        User = ExtractUserInformation();
        LogUserInformation();
    }

    private AuthenticatedUserDto ExtractUserInformation()
    {
        var userDto = new AuthenticatedUserDto();

        // Extract basic user information from headers
        if (Request.Headers.TryGetValue("X-MS-CLIENT-PRINCIPAL-ID", out var userId))
        {
            userDto.UserId = userId.ToString();
        }

        if (Request.Headers.TryGetValue("X-MS-CLIENT-PRINCIPAL-NAME", out var userName))
        {
            userDto.Name = userName.ToString();
        }

        if (Request.Headers.TryGetValue("X-MS-CLIENT-PRINCIPAL-IDP", out var idp))
        {
            userDto.IdentityProvider = idp.ToString();
        }

        // Parse token expiration
        if (Request.Headers.TryGetValue("X-MS-TOKEN-AAD-EXPIRES-ON", out var expiresOn))
        {
            if (long.TryParse(expiresOn.ToString(), out var timestamp))
            {
                userDto.TokenExpiresOn = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            }
        }

        // Parse the full principal information
        if (Request.Headers.TryGetValue("X-MS-CLIENT-PRINCIPAL", out var principalHeader))
        {
            try
            {
                var principalJson = Encoding.UTF8.GetString(Convert.FromBase64String(principalHeader.ToString()));
                userDto.Principal = JsonSerializer.Deserialize<ClientPrincipal>(principalJson, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to parse X-MS-CLIENT-PRINCIPAL header: {Error}", ex.Message);
            }
        }

        // Set authentication status
        userDto.IsAuthenticated = !string.IsNullOrEmpty(userDto.UserId);

        // Collect all authentication-related headers for debugging
        var authHeaderNames = new[]
        {
            "X-MS-CLIENT-PRINCIPAL",
            "X-MS-CLIENT-PRINCIPAL-ID",
            "X-MS-CLIENT-PRINCIPAL-NAME",
            "X-MS-CLIENT-PRINCIPAL-IDP",
            "X-MS-TOKEN-AAD-ID-TOKEN",
            "X-MS-TOKEN-AAD-ACCESS-TOKEN",
            "X-MS-TOKEN-AAD-REFRESH-TOKEN",
            "X-MS-TOKEN-AAD-EXPIRES-ON",
            "X-MS-CLIENT-AUTHORIZATION"
        };

        foreach (var headerName in authHeaderNames)
        {
            if (Request.Headers.TryGetValue(headerName, out var headerValue))
            {
                // Mask sensitive tokens for security
                if (headerName.Contains("TOKEN") || headerName.Contains("AUTHORIZATION") || headerName.Contains("PRINCIPAL"))
                {
                    userDto.AuthHeaders[headerName] = MaskSensitiveValue(headerValue.ToString());
                }
                else
                {
                    userDto.AuthHeaders[headerName] = headerValue.ToString();
                }
            }
        }

        return userDto;
    }

    private void LogUserInformation()
    {
        _logger.LogInformation("=== Current User Information ===");
        _logger.LogInformation("User ID: {UserId}", User.UserId ?? "Not available");
        _logger.LogInformation("User Name: {UserName}", User.Name ?? "Not available");
        _logger.LogInformation("Identity Provider: {IdentityProvider}", User.IdentityProvider ?? "Not available");
        _logger.LogInformation("Is Authenticated: {IsAuthenticated}", User.IsAuthenticated);
        
        if (User.TokenExpiresOn.HasValue)
        {
            _logger.LogInformation("Token Expires On: {TokenExpiration}", User.TokenExpiresOn.Value);
        }

        if (User.Principal?.Claims?.Any() == true)
        {
            _logger.LogInformation("User Claims Count: {ClaimsCount}", User.Principal.Claims.Count);
            foreach (var claim in User.Principal.Claims)
            {
                _logger.LogInformation("Claim - Type: {ClaimType}, Value: {ClaimValue}", 
                    claim.Typ, 
                    claim.Typ?.Contains("email", StringComparison.OrdinalIgnoreCase) == true ? claim.Val : MaskSensitiveValue(claim.Val));
            }
        }

        _logger.LogInformation("=== End User Information ===");
    }

    private static string MaskSensitiveValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "[Empty]";
        }

        // Show first and last few characters, mask the middle
        if (value.Length <= 10)
        {
            return "[MASKED]";
        }

        return $"{value[..4]}...{value[^4..]}";
    }
}
