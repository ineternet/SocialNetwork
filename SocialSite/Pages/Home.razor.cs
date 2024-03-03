using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SocialSite.Models;
using SocialSite.Services;

namespace SocialSite.Pages;

public partial class Home : ComponentBase
{
    private readonly CreatePostModel CreatePost = new();
    private readonly UserUpdateModel UserUpdate = new();
    private readonly List<Post> PostIndex = [];
    protected bool AttachEnabled { get; set; }
    protected bool EditingBio { get; set; }
    protected InputText? BioEditText;

    [Inject]
    private AuthService Auth { get; init; }
    [Inject]
    private NavigationManager NavigationManager { get; init; }
    [Inject]
    private AggregatorService Homepage { get; init; }
    [Inject]
    private UsersService Users { get; init; }
    [Inject]
    private PostsService Posts { get; init; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (this.BioEditText is not null && this.EditingBio)
        {
            await this.BioEditText!.Element!.Value.FocusAsync();
        }
        if (this.Auth.User is not null || !firstRender)
            return;
        if (!await this.Auth.AuthenticateAsync())
        {
            this.NavigationManager.NavigateTo("/n/me");
            return;
        }
        await foreach (var post in await this.Homepage.AggregateAsync())
        {
            if (post.Parent is null)
            {
                this.PostIndex.Add(post);
            }
        }
        this.StateHasChanged();
    }

    protected Task ClickAttachBtn()
    {
        this.AttachEnabled = !this.AttachEnabled;
        return Task.CompletedTask;
    }

    protected Task ClickStatusEdit()
    {
        this.EditingBio = !this.EditingBio;
        this.StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task HandleValidSubmit()
    {
        if (this.Auth.User is not User author)
            return;
        var NewPost = await this.CreatePost.ToPostAsync(author);
        if (NewPost is not null)
        {
            await this.Posts.AttachThenInsertAsync(NewPost, author);
            this.NavigationManager.NavigateTo($"/post/{NewPost.PostId}");
        }
    }

    private async Task HandleBioSubmit()
    {
        if (this.Auth.User is not User user)
            return;
        await this.Users.ApplyUpdateAsync(user, this.UserUpdate);
        this.EditingBio = false;
        this.UserUpdate.Bio = null;
        this.StateHasChanged();
    }
}
