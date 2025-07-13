using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;

namespace WebApp.Pages.Users;

public class MsGraphUsers : PageModel
{
    public ICollection<MSGraphUser> Users { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var credential = new ChainedTokenCredential(
            new ManagedIdentityCredential(),
            new EnvironmentCredential());

        string[] scopes = { "https://graph.microsoft.com/.default" };

        using var graphServiceClient = new GraphServiceClient(credential, scopes);

        ICollection<MSGraphUser>? msGraphUsers = [];

        try
        {
            var users = await graphServiceClient.Users.GetAsync();

            msGraphUsers = users?.Value?.ConvertAll(u => new MSGraphUser
            {
                UserPrincipalName = u.UserPrincipalName,
                DisplayName = u.DisplayName,
                Mail = u.Mail,
                JobTitle = u.JobTitle
            });
        }
        catch (Exception ex)
        {
            string msg = ex.Message;
        }

        Users = msGraphUsers ?? [];
    }
}
