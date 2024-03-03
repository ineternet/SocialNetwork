using Microsoft.AspNetCore.Components;
using SocialSite.Services;

namespace SocialSite.Shared;

public partial class MainLayout : LayoutComponentBase
{
    protected bool ShowMenu { get; set; }

    [Inject]
    private AuthService Auth { get; init; }

    protected override async Task OnAfterRenderAsync(bool first)
    {
        if (!first)
            return;

        await this.Auth.AuthenticateAsync();
        this.StateHasChanged();
    }
}
