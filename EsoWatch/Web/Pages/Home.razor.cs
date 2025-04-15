using EsoWatch.Data;
using EsoWatch.Data.Entities;
using EsoWatch.Web.Dialogs;

using Microsoft.EntityFrameworkCore;

using Radzen;

namespace EsoWatch.Web.Pages;

public partial class Home : IDisposable
{
    private readonly IDbContextFactory<EsoDbContext> _dbContextFactory;
    private readonly DialogService _dialogService;
    private readonly List<EsoCharacter> _characters = [];
    private readonly List<GenericTimer> _timers = [];

    private readonly CancellationTokenSource _cts = new();

    public Home(IDbContextFactory<EsoDbContext> dbContextFactory, DialogService dialogService)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
    }

    protected override async Task OnInitializedAsync()
    {
        await RefreshTimersAsync();
        _ = RefreshPeriodicallyAsync(_cts.Token);
    }

    private async Task RefreshTimersAsync(CancellationToken cancellationToken = default)
    {
        await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        _characters.Clear();
        _characters.AddRange(await dbContext.Characters.OrderBy(c => c.Name).ToListAsync(cancellationToken: cancellationToken));
        _timers.Clear();
        _timers.AddRange(await dbContext.Timers.OrderBy(t => t.ElapsesAt).ToListAsync(cancellationToken: cancellationToken));
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

    private async Task AddNewCharacter()
    {
        dynamic? result = await _dialogService.OpenAsync<AddCharacterDialog>("Add new character");
        if (result is AddCharacterDialog.Model model)
        {
            await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
            var character = new EsoCharacter
            {
                Name = model.Name,
            };
            dbContext.Characters.Add(character);
            await dbContext.SaveChangesAsync();
            await RefreshTimersAsync();
        }
    }

    private async Task AddNewTimer()
    {
        dynamic? result = await _dialogService.OpenAsync<AddTimerDialog>("Add new timer");
        if (result is AddTimerDialog.Model model)
        {
            await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
            var timer = new GenericTimer
            {
                Name = model.Name,
                Duration = model.GetTimeLeft()!.Value,
                ElapsesAt = DateTime.UtcNow + model.GetTimeLeft()!.Value,
            };
            dbContext.Timers.Add(timer);
            await dbContext.SaveChangesAsync();
            await RefreshTimersAsync();
        }
    }
}