using Microsoft.AspNetCore.Components;
using SocialSite.Models;
using SocialSite.Services;

namespace SocialSite.Pages;

public partial class PostPage : ComponentBase
{
    [Parameter]
    public Post? PostData { get; set; }

    private bool? RouteReturnedNull { get; set; }

    [Parameter]
    public string? PostId { get; set; }

    [Inject]
    private PostsService Posts { get; init; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;
        if (int.TryParse(this.PostId, out var postid))
        {
            Post? post = await this.Posts.GetByIdAsync(postid, Post.DefaultNavigation);
            if ((this.PostData = post) is null)
                this.RouteReturnedNull = true;
        }
        else
        {
            this.RouteReturnedNull = true;
        }
        this.StateHasChanged();
    }
}
