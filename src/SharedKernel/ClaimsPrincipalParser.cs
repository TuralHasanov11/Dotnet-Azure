using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SharedKernel;

public static class ClaimsPrincipalParser
{
    private static readonly JsonSerializerOptions JsonSerializeOptions = new() { PropertyNameCaseInsensitive = true };

    public class ClientPrincipalClaim
    {
        [JsonPropertyName("typ")]
        public string Type { get; set; }
        [JsonPropertyName("val")]
        public string Value { get; set; }
    }

    public class ClientPrincipal
    {
        [JsonPropertyName("auth_typ")]
        public string IdentityProvider { get; init; }
        [JsonPropertyName("name_typ")]
        public string NameClaimType { get; init; }
        [JsonPropertyName("role_typ")]
        public string RoleClaimType { get; init; }
        [JsonPropertyName("claims")]
        public IEnumerable<ClientPrincipalClaim> Claims { get; set; }
    }

    public static ClaimsPrincipal Parse(HttpRequest req)
    {
        var principal = new ClientPrincipal();

        if (req.Headers.TryGetValue("x-ms-client-principal", out var header))
        {
            var data = header[0];
            var decoded = Convert.FromBase64String(data);
            var json = Encoding.UTF8.GetString(decoded);
            principal = JsonSerializer.Deserialize<ClientPrincipal>(json, JsonSerializeOptions);
        }

        return new ClaimsPrincipal(
            new ClaimsIdentity(
                principal?.Claims.Select(c => new Claim(c.Type, c.Value)),
                principal?.IdentityProvider,
                principal?.NameClaimType,
                principal?.RoleClaimType
            )
        );
    }

    public static void LogUserInformation<T>(this ClaimsPrincipal User, ILogger<T> _logger)
    {
        _logger.LogInformation("=== Current User Information ===");
        _logger.LogInformation("User ID: {UserId}", User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "Not available");
        _logger.LogInformation("User Name: {UserName}", User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Not available");
        _logger.LogInformation("Identity Provider: {IdentityProvider}", User.Claims.FirstOrDefault(c => c.Type == "idp")?.Value ?? "Not available");
        _logger.LogInformation("Is Authenticated: {IsAuthenticated}", User.Identity?.IsAuthenticated);

        if (User?.Claims?.Any() == true)
        {
            _logger.LogInformation("User Claims Count: {ClaimsCount}", User.Claims.Count());
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation("Claim - Type: {ClaimType}, Value: {ClaimValue}",
                    claim.Type,
                    claim.Type?.Contains("email", StringComparison.OrdinalIgnoreCase) == true ? claim.Value : MaskSensitiveValue(claim.Value));
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
