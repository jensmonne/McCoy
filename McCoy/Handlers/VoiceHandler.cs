using Discord.WebSocket;

namespace McCoy.Handlers;

public static class VoiceHandler
{
    private static readonly ulong LogChannelId = 1378141651301695538;
    
    private static readonly Dictionary<ulong, DateTime> VoiceJoinTimes = new();

    public static async Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        if (user.IsBot) return;
        
        var guildUser = user as SocketGuildUser;
        var guild = guildUser?.Guild;
        if (guild == null) return;
        
        var logChannel = guild.GetTextChannel(LogChannelId);
        if (logChannel == null) return;

        string message;
        
        // JOIN
        if (before.VoiceChannel == null && after.VoiceChannel != null)
        {
            VoiceJoinTimes[user.Id] = DateTime.UtcNow;
            message = $"**{user.Username}** joined **{after.VoiceChannel.Name}**.";
        }
        // LEAVE
        else if (before.VoiceChannel != null && after.VoiceChannel == null)
        {
            var joinTime = VoiceJoinTimes.ContainsKey(user.Id) ? VoiceJoinTimes[user.Id] : (DateTime?)null;
            VoiceJoinTimes.Remove(user.Id);

            var timeSpent = joinTime.HasValue ? FormatDuration(DateTime.UtcNow - joinTime.Value) : "unknown";
            message = $"**{user.Username}** left **{before.VoiceChannel.Name}** (Time spent: {timeSpent}).";
        }
        // SWITCH
        else if (before.VoiceChannel != after.VoiceChannel)
        {
            message = $"**{user.Username}** switched from **{before.VoiceChannel.Name}** to **{after.VoiceChannel.Name}**.";
        }
        else
        {
            // ignore mute and unmute
            return;
        }

        await logChannel.SendMessageAsync(message);
    }

    private static string FormatDuration(TimeSpan span)
    {
        return $"{(int)span.TotalMinutes}m {span.Seconds}s";
    }
}