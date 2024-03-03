using Microsoft.AspNetCore.Components;
using SocialSite.Models;
using SocialSite.Services;

namespace SocialSite.Shared;

public partial class PostDisplay : ComponentBase
{
    protected bool ReplyEnabled { get; set; }
    protected bool AttachEnabled { get; set; }
    protected CreatePostModel CreatePost = new();

    [Parameter]
    public Post? Post { get; set; }
    [Parameter]
    public bool? NotFound { get; set; }
    [Parameter]
    public bool? Inline { get; set; }
    [Parameter]
    public int? Depth { get; set; }
    protected int GetDepth() => this.Depth ?? (this.Post?.Parent is null ? 0 : 1);
    [Parameter]
    public int? MaxDepth { get; set; } = 10;

    [Inject]
    private AuthService Auth { get; init; }
    [Inject]
    private NavigationManager NavigationManager { get; init; }
    [Inject]
    private PostsService Posts { get; init; }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (this.NotFound ?? false)
            return Task.CompletedTask;
        this.CreatePost.Parent = this.Post;
        this.StateHasChanged();
        return Task.CompletedTask;
    }

    protected async Task ClickLikeBtn()
    {
        if (this.Post is null || (this.NotFound ?? false) || this.Auth.User is null)
            return;
        if (this.Post.Likes.Any(x => x == this.Auth.User))
        {
            await this.Posts.DeleteLikeAsync(this.Post, this.Auth.User);
        }
        else
        {
            await this.Posts.PutLikeAsync(this.Post, this.Auth.User);
        }
        this.StateHasChanged();
    }

    protected void ClickReplyBtn()
    {
        if (this.CreatePost.Parent is null)
            return;
        this.ReplyEnabled = !this.ReplyEnabled;
    }

    protected async Task ClickDeleteBtn()
    {
        if (this.Auth.User is User author && this.Post?.Author == author)
        {
            await this.Posts.DeletePostAsync(this.Post);
            this.Post = null;
            this.NotFound = true;
            this.StateHasChanged();
        }
    }

    protected Task ClickContextMenuBtn() => Task.CompletedTask;

    protected Task ClickAttachBtn()
    {
        this.AttachEnabled = !this.AttachEnabled;
        return Task.CompletedTask;
    }

    protected async Task HandleValidReply()
    {
        if (this.CreatePost.Parent is not Post parent || parent != this.Post || this.Auth.User is not User author)
            return;
        var NewPost = await this.CreatePost.ToPostAsync(author);
        if (NewPost is not null)
        {
            await this.Posts.AttachThenInsertAsync(NewPost, parent, author);
            this.NavigationManager.NavigateTo($"/post/{NewPost.PostId}");
        }
    }
}
