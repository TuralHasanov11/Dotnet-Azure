using System.Collections.ObjectModel;

namespace WebApp.Pages.Users;

/// <summary>
/// DTO for Azure App Service authenticated user information
/// </summary>
public class AuthenticatedUserDto
{
    /// <summary>
    /// User's unique identifier (from X-MS-CLIENT-PRINCIPAL-ID)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// User's display name or email (from X-MS-CLIENT-PRINCIPAL-NAME)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Identity provider used for authentication (from X-MS-CLIENT-PRINCIPAL-IDP)
    /// </summary>
    public string? IdentityProvider { get; set; }

    /// <summary>
    /// Full principal information (decoded from X-MS-CLIENT-PRINCIPAL)
    /// </summary>
    public ClientPrincipal? Principal { get; set; }

    /// <summary>
    /// Token expiration time (from X-MS-TOKEN-AAD-EXPIRES-ON)
    /// </summary>
    public DateTimeOffset? TokenExpiresOn { get; set; }

    /// <summary>
    /// Indicates if the user is authenticated
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// Raw authentication headers for debugging
    /// </summary>
    public Dictionary<string, string> AuthHeaders { get; }
}

/// <summary>
/// Represents the client principal information from Azure App Service
/// </summary>
public class ClientPrincipal
{
    /// <summary>
    /// Authentication type
    /// </summary>
    public string? Auth_typ { get; set; }

    /// <summary>
    /// Name claim type
    /// </summary>
    public string? Name_typ { get; set; }

    /// <summary>
    /// Role claim type
    /// </summary>
    public string? Role_typ { get; set; }

    /// <summary>
    /// User claims
    /// </summary>
    public ReadOnlyCollection<ClientPrincipalClaim> Claims { get; set; }
}

/// <summary>
/// Represents a claim in the client principal
/// </summary>
public class ClientPrincipalClaim
{
    /// <summary>
    /// Claim type
    /// </summary>
    public string? Typ { get; set; }

    /// <summary>
    /// Claim value
    /// </summary>
    public string? Val { get; set; }
}
