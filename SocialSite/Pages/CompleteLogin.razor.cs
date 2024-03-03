using Microsoft.AspNetCore.Components;
using SocialSite.Services;

namespace SocialSite.Pages;

public partial class CompleteLogin : ComponentBase
{
    [Parameter]
    public string? RequestId { get; set; }
    [Parameter]
    public string? RequestSecret { get; set; }

    protected bool? AuthStatus { get; set; }

    [Inject]
    private AuthService Auth { get; init; }
    [Inject]
    private NavigationManager NavigationManager { get; init; }

    protected override async Task OnAfterRenderAsync(bool first)
    {
        if (!first)
            return;
        if (this.RequestSecret is string secret && Guid.TryParse(this.RequestId, out var RequestGuid))
        {
            this.AuthStatus = await this.Auth.CompleteLoginAsync(RequestGuid, secret, this.NavigationManager);
            if (this.AuthStatus == true)
                return;
        }
        this.NavigationManager.NavigateTo("/n/complete");
    }
}
