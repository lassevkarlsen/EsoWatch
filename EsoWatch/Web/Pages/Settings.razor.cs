using EsoWatch.Data;
using EsoWatch.Data.Entities;

using LasseVK.Core;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace EsoWatch.Web.Pages;

public partial class Settings : ComponentBase
{
    private readonly IDbContextFactory<EsoDbContext> _dbContextFactory;
    private readonly NavigationManager _navigationManager;

    [Parameter]
    public Guid UserId { get; set; }

    private Model? _model;

    public Settings(IDbContextFactory<EsoDbContext> dbContextFactory, NavigationManager navigationManager)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
    }

    protected override async Task OnParametersSetAsync()
    {
        await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
        UserSettings? settings = await dbContext.UserSettings.FirstOrDefaultAsync(s => s.UserId == UserId);
        _model = new Model
        {
            Settings = settings ?? UserSettings.DefaultForUser(UserId),
            IsNew = settings == null,
        };
    }

    private async Task SaveSettings()
    {
        Assume.That(_model != null);

        await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Attach(_model.Settings);

        dbContext.Entry(_model.Settings).State = _model.IsNew ? EntityState.Added : EntityState.Modified;

        await dbContext.SaveChangesAsync();

        _navigationManager.NavigateTo($"/{UserId}");
    }

    private void Cancel()
    {
        _navigationManager.NavigateTo($"/{UserId}");
    }
}