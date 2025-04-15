using EsoWatch.Data;
using EsoWatch.Data.Entities;

using LasseVK.Core;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

using Radzen;
using Radzen.Blazor;

namespace EsoWatch.Web.Components;

public partial class RenderCharacter : ComponentBase
{
    private readonly IDbContextFactory<EsoDbContext> _dbContextFactory;
    private readonly DialogService _dialogService;

    [Parameter]
    public EsoCharacter? Character { get; set; }

    public RenderCharacter(IDbContextFactory<EsoDbContext> dbContextFactory, DialogService dialogService)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
    }

    private async Task CharacterLootedAsync()
    {
        await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Attach(Character!);
        Character!.DungeonRewardsAvailableAt = DateTime.UtcNow.AddHours(20);
        await dbContext.SaveChangesAsync();
    }

    private string TimeLeft() => TimeLeftFormatter.Format(Character!.DungeonRewardsAvailableAt!.Value - DateTime.UtcNow);

    private async Task DeleteCharacterAsync()
    {
        Assume.That(Character != null);
        bool? confirmed = await _dialogService.Confirm($"Are you sure you want to delete this character?", $"Delete character {Character.Name}", new ConfirmOptions
        {
            OkButtonText = "Yes", CancelButtonText = "No",
        });

        if (confirmed == true)
        {
            await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.Attach(Character!);
            dbContext.Characters.Remove(Character!);
            await dbContext.SaveChangesAsync();

            Character = null;
        }
    }

    private async Task ResetTimerAsync()
    {
        Assume.That(Character != null);
        bool? confirmed = await _dialogService.Confirm($"Are you sure you want to stop and reset the timer for this character?", $"Reset timer for {Character.Name}", new ConfirmOptions
        {
            OkButtonText = "Yes", CancelButtonText = "No",
        });

        if (confirmed == true)
        {
            await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.Attach(Character);
            Character.DungeonRewardsAvailableAt = null;
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task OnClick(RadzenSplitButtonItem? arg)
    {
        switch (arg?.Value)
        {
            case null:
                await CharacterLootedAsync();
                break;

            case "delete":
                await DeleteCharacterAsync();
                break;

            case "reset":
                await ResetTimerAsync();
                break;
        }

        StateHasChanged();
    }
}