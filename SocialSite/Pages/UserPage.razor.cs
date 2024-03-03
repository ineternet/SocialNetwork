using Microsoft.AspNetCore.Components;
using SocialSite.Services;

namespace SocialSite.Pages;

public partial class UserPage : ComponentBase
{
    private User? PageUser { get; set; }
    private readonly List<Post> PostIndex = [];
    private readonly List<Post> ReplyIndex = [];

    [Parameter]
    public string? UserId { get; set; }
    [Parameter]
    public string? Subpage { get; set; }

    [Inject]
    private UsersService Users { get; init; }

    protected override async Task OnInitializedAsync()
    {
        if (this.UserId is null)
            return;
        if (!int.TryParse(this.UserId, out var userid))
            return;
        this.PageUser = await this.Users.GetByIdAsync(userid, User.DefaultNavigation);

        if (this.PageUser is not null)
        {
            await foreach (var post in await this.Users.GetRootIndexAsync(this.PageUser))
            {
                this.PostIndex.Add(post);
            }

            await foreach (var reply in await this.Users.GetReplyIndexAsync(this.PageUser))
            {
                this.ReplyIndex.Add(reply);
            }

            this.StateHasChanged();
        }
    }
}
