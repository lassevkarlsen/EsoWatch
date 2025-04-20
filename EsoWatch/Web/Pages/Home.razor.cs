using EsoWatch.Data;
using EsoWatch.Data.Entities;
using EsoWatch.Web.Dialogs;

using LasseVK.Core;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

using Radzen;

namespace EsoWatch.Web.Pages;

public partial class Home : IDisposable
{
    [Parameter]
    public required Guid? UserId { get; set; }

    private readonly Guid _freshUserId = Guid.NewGuid();

    private readonly IDbContextFactory<EsoDbContext> _dbContextFactory;
    private readonly DialogService _dialogService;
    private readonly NavigationManager _navigationManager;
    private readonly IConfiguration _configuration;
    private readonly List<GenericTimer> _timers = [];
    private readonly string _siteUrl;

    private readonly CancellationTokenSource _cts = new();

    private UserSettings? _userSettings;

    public Home(IDbContextFactory<EsoDbContext> dbContextFactory, DialogService dialogService, NavigationManager navigationManager, IConfiguration configuration)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _siteUrl = _configuration["SiteUrl"] ?? "http://localhost:5176/";
    }

    protected override async Task OnParametersSetAsync()
    {
        if (UserId != null)
        {
            await LoadUserSettingsAsync();
            await RefreshTimersAsync();
            _ = RefreshPeriodicallyAsync(_cts.Token);
        }
    }

    private async Task LoadUserSettingsAsync()
    {
        Assert.That(UserId != null);
        await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
        _userSettings = await dbContext.UserSettings.FirstOrDefaultAsync(s => s.UserId == UserId) ?? UserSettings.DefaultForUser(UserId.Value);
    }

    private async Task RefreshTimersAsync(CancellationToken cancellationToken = default)
    {
        Assert.That(UserId != null);
        await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        _timers.Clear();
        _timers.AddRange(await dbContext.Timers.Where(t => t.UserId == UserId).OrderBy(t => t.ElapsesAt).ThenBy(t => t.Duration).ToListAsync(cancellationToken: cancellationToken));
    }

    private async Task RefreshPeriodicallyAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                await RefreshTimersAsync(cancellationToken);
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (TaskCanceledException)
        {
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }

    private async Task AddNewTimer()
    {
        Assume.That(UserId != null);

        dynamic? result = await _dialogService.OpenAsync<AddTimerDialog>("Add new timer", new Dictionary<string, object>
        {
            {
                "UserId", UserId.Value
            },
        });
        if (result is AddTimerDialog.Model model)
        {
            await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
            var timer = new GenericTimer
            {
                Name = model.Name,
                Duration = model.GetTimeLeft()!.Value,
                ElapsesAt = DateTime.UtcNow + model.GetTimeLeft()!.Value,
                UserId = UserId.Value,
            };
            dbContext.Timers.Add(timer);
            await dbContext.SaveChangesAsync();
            await RefreshTimersAsync();
        }
    }

    private void Configure()
    {
        Assert.That(UserId != null);
        _navigationManager.NavigateTo($"settings/{UserId}");
    }
}