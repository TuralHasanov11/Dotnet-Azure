using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedKernel;

namespace WebApp.Pages.Users;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public ClaimsPrincipal User { get; private set; } = new();

    public void OnGet(HttpContext httpContext)
    {
        User = ClaimsPrincipalParser.Parse(httpContext.Request);
        User.LogUserInformation(_logger);
    }
}
