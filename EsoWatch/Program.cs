using EsoWatch.Data;
using EsoWatch.Web;
using EsoWatch.Workers;

using LasseVK.Configuration;
using LasseVK.Pushover;

using Microsoft.EntityFrameworkCore;

using Radzen;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddStandardConfigurationSources<Program>();
builder.Services.AddDbContextFactory<EsoDbContext>();

builder.Services.AddPushoverClient(options =>
{
    options.DefaultUser = builder.Configuration["Pushover:UserKey"];
    options.ApiToken = builder.Configuration["Pushover:ApiToken"];
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRadzenComponents();
builder.Services.AddHostedService<NotificationWorker>();

WebApplication host = builder.Build();

// Configure the HTTP request pipeline.
if (!host.Environment.IsDevelopment())
{
    host.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // host.UseHsts();
}

host.UseHttpsRedirection();

{
    using IServiceScope serviceScope = host.Services.CreateScope();
    IDbContextFactory<EsoDbContext> dbContextFactory = serviceScope.ServiceProvider.GetRequiredService<IDbContextFactory<EsoDbContext>>();

    await using EsoDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
    await dbContext.Database.MigrateAsync();
}

// host.UseAntiforgery();

host.MapStaticAssets();
host.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

host.Run();