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

        var usersWithPushover = (await dbContext.UserSettings.Where(u => u.UsePushover && u.PushoverUserKey != null && u.PushoverUserKey != "").ToListAsync(stoppingToken)).ToDictionary(u => u.UserId);

        DateTime now = DateTime.UtcNow;
        List<GenericTimer> elapsedTimers = await dbContext.Timers.Where(t => t.ElapsesAt != null && t.ElapsesAt < now && !t.NotificationSent && t.Remaining == null).ToListAsync(stoppingToken);
        foreach (GenericTimer timer in elapsedTimers)
        {
            timer.ElapsesAt = null;
            timer.NotificationSent = true;

            if (!usersWithPushover.TryGetValue(timer.UserId, out UserSettings? userSettings))
            {
                continue;
            }

            var notification = new PushoverNotification
            {
                Message = $"Timer '{timer.Name}' has elapsed!",
                TargetUser = userSettings.PushoverUserKey,
            };
            await dbContext.SaveChangesAsync(stoppingToken);
            await _pushover.SendAsync(notification, stoppingToken);
        }

        List<EsoCharacter> elapsedCharacterTimers = await dbContext.Characters.Where(c => c.DungeonRewardsAvailableAt != null && c.DungeonRewardsAvailableAt <= now && !c.NotificationSent).ToListAsync(stoppingToken);
        foreach (EsoCharacter character in elapsedCharacterTimers)
        {
            character.NotificationSent = true;

            if (!usersWithPushover.TryGetValue(character.UserId, out UserSettings? userSettings))
            {
                continue;
            }

            var notification = new PushoverNotification
            {
                Message = $"Character '{character.Name}' can get dungeon bonus loot now!",
                TargetUser = userSettings.PushoverUserKey,
            };
            await dbContext.SaveChangesAsync(stoppingToken);
            await _pushover.SendAsync(notification, stoppingToken);
        }
    }
}