using Microsoft.AspNetCore.Components;
using SocialSite.Models;
using SocialSite.Services;

namespace SocialSite.Pages;

public partial class Index : ComponentBase
{
    [Parameter]
    public string? ShowActionErr { get; set; }

    private LoginModel LoginModel { get; init; } = new();
    private string? OutLink { get; set; }

    [Inject]
    private AuthService Auth { get; init; }
    [Inject]
    private NavigationManager NavigationManager { get; init; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (await this.Auth.UserAsync() is not null)
        {
            this.NavigationManager.NavigateTo("/me");
        }
    }
    
    public async Task HandleValidSubmit()
    {
        if (this.LoginModel.LoginIdentifier is not string ident)
            return;
        Guid rid = await this.Auth.StartLoginAsync(ident);
    }
}
