using EsoWatch.Data;
using EsoWatch.Data.Entities;

using LasseVK.Pushover;

using Microsoft.EntityFrameworkCore;

namespace EsoWatch.Workers;

public class NotificationWorker : BackgroundService
{
    private readonly IDbContextFactory<EsoDbContext> _dbContextFactory;
    private readonly IPushover _pushover;

    public NotificationWorker(IDbContextFactory<EsoDbContext> dbContextFactory, IPushover pushover)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        _pushover = pushover ?? throw new ArgumentNullException(nameof(pushover));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.Hour == 23 || DateTime.Now.Hour < 8)
                {
                    await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
                    continue;
                }

                await CheckTimersAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
        }
    }

    private async Task CheckTimersAsync(CancellationToken stoppingToken)
    {
        await using EsoDbContext dbContext = await _dbContextFactory.CreateDbContextAsync(stoppingToken);

        DateTime now = DateTime.UtcNow;
        foreach (GenericTimer timer in await dbContext.Timers.ToListAsync(stoppingToken))
        {
            if (timer.ElapsesAt < now && !timer.NotificationSent)
            {
                await _pushover.SendAsync($"Timer '{timer.Name}' has elapsed!", stoppingToken);
                timer.ElapsesAt = null;
                timer.NotificationSent = true;
            }
        }

        foreach (EsoCharacter character in await dbContext.Characters.ToListAsync(stoppingToken))
        {
            if (character.DungeonRewardsAvailableAt != null && character.DungeonRewardsAvailableAt < now)
            {
                await _pushover.SendAsync($"Character '{character.Name}' can get dungeon bonus loot now!", stoppingToken);
                character.DungeonRewardsAvailableAt = null;
            }
        }

        await dbContext.SaveChangesAsync(stoppingToken);
    }
}