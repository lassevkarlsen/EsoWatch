using EsoWatch.Data;
using EsoWatch.Data.Entities;

using LasseVK.Core;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

using Radzen;
using Radzen.Blazor;

namespace EsoWatch.Web.Components;

public partial class RenderTimer : ComponentBase
{
    private readonly IDbContextFactory<EsoDbContext> _dbContextFactory;
    private readonly DialogService _dialogService;

    [Parameter]
    public GenericTimer? Timer { get; set; }

    public RenderTimer(IDbContextFactory<EsoDbContext> dbContextFactory, DialogService dialogService)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
    }

    private string TimeLeft() => TimeLeftFormatter.Format(Timer!.ElapsesAt!.Value - DateTime.UtcNow);

    private async Task DeleteTimerAsync()
    {
        Assume.That(Timer != null);
        bool? confirmed = await _dialogService.Confirm($"Are you sure you want to delete the timer '{Timer.Name}?", $"Delete timer '{Timer.Name}", new ConfirmOptions
        {
            OkButtonText = "Yes", CancelButtonText = "No",
        });

        if (confirmed == true)
        {
            await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.Attach(Timer!);
            dbContext.Timers.Remove(Timer!);
            await dbContext.SaveChangesAsync();

            Timer = null;
        }
    }

    private async Task OnClick(RadzenSplitButtonItem? arg)
    {
        Assume.That(Timer != null);

        switch (arg?.Value)
        {
            case null:
                if (Timer.ElapsesAt == null)
                {
                    await RestartTimerAsync();
                }

                break;

            case "stop":
                await StopTimerAsync();
                break;

            case "restart":
                await RestartTimerAsync();
                break;

            case "delete":
                await DeleteTimerAsync();
                break;
        }
        StateHasChanged();
    }

    private async Task RestartTimerAsync()
    {
        Assume.That(Timer != null);

        await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Attach(Timer);
        Timer.ElapsesAt = DateTime.UtcNow + Timer.Duration;
        Timer.NotificationSent = false;
        await dbContext.SaveChangesAsync();
    }

    private async Task StopTimerAsync()
    {
        Assume.That(Timer != null);

        await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Attach(Timer);
        Timer.ElapsesAt = null;
        Timer.NotificationSent = true;
        await dbContext.SaveChangesAsync();
    }
}