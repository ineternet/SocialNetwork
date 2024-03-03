using Blazored.LocalStorage;
using Microsoft.EntityFrameworkCore;
using SocialModel;
using SocialSite.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddDbContextFactory<DbContext, SocialContext.Factory>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddTransient<AggregatorService>();
builder.Services.AddTransient<PostsService>();
builder.Services.AddTransient<UsersService>();
builder.WebHost.UseStaticWebAssets();

WebApplication app = builder.Build();
app.UseHttpsRedirection();
app.UseHsts();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await using (var scope = app.Services.CreateAsyncScope())
{
    await using var dbContext = await scope.ServiceProvider.GetRequiredService<IDbContextFactory<DbContext>>().CreateDbContextAsync();
    await dbContext.Database.MigrateAsync();
}

app.Run();
