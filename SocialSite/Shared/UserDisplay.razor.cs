using Microsoft.AspNetCore.Components;
using SocialSite.Services;

namespace SocialSite.Shared;

public partial class UserDisplay : ComponentBase
{
    [Parameter]
    public User? User { get; set; }
    [Parameter]
    public bool? NotFound { get; set; }
}
