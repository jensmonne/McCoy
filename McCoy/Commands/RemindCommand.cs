using System.Collections.Concurrent;
using Discord;
using Discord.Interactions;

namespace McCoy.Commands;

public class RemindCommand : InteractionModuleBase<SocketInteractionContext>
{
    private static readonly ConcurrentDictionary<Guid, CancellationTokenSource> Reminders = new();

    [SlashCommand("remind", "Set a reminder.")]
    public async Task RemindAsync(string time, string message)
    {
        if (!TryParseTime(time, out var delay))
        {
            await RespondAsync("⚠️ Invalid time format! Use s/m/h (e.g. 10m, 30s, 2h).", ephemeral: true);
            return;
        }

        await RespondAsync($"Okay, I’ll remind you in {time}.", ephemeral: true);

        var tokenSource = new CancellationTokenSource();
        var id = Guid.NewGuid();
        Reminders[id] = tokenSource;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delay, tokenSource.Token);

                await Context.User.SendMessageAsync($"Reminder: {message}");
                Reminders.TryRemove(id, out _);
            }
            catch (TaskCanceledException) { /* reminder cancelled */ }
        });
    }

    private bool TryParseTime(string input, out TimeSpan time)
    {
        time = TimeSpan.Zero;
        if (string.IsNullOrWhiteSpace(input)) return false;

        try
        {
            char unit = input[^1];
            double value = double.Parse(input[..^1]);

            time = unit switch
            {
                's' => TimeSpan.FromSeconds(value),
                'm' => TimeSpan.FromMinutes(value),
                'h' => TimeSpan.FromHours(value),
                _ => TimeSpan.Zero
            };

            return time > TimeSpan.Zero;
        }
        catch
        {
            return false;
        }
    }
}