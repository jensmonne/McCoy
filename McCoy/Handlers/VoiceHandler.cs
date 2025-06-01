using Discord;
using Discord.WebSocket;
using McCoy.Utilities;

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

        var embed = new EmbedBuilder()
            .WithAuthor(user);

        // JOIN
        if (before.VoiceChannel == null && after.VoiceChannel != null)
        {
            VoiceJoinTimes[user.Id] = DateTime.UtcNow;

            embed.WithTitle("Voice Channel Join")
                 .WithColor(Color.Blue)
                 .AddField("User", $"<@{user.Id}>", true)
                 .AddField("Channel", after.VoiceChannel.Name, true);
        }
        // LEAVE
        else if (before.VoiceChannel != null && after.VoiceChannel == null)
        {
            var joinTime = VoiceJoinTimes.ContainsKey(user.Id) ? VoiceJoinTimes[user.Id] : (DateTime?)null;
            VoiceJoinTimes.Remove(user.Id);

            var timeSpent = joinTime.HasValue
                ? EmbedUtils.FormatDuration(DateTime.UtcNow - joinTime.Value)
                : "unknown";

            embed.WithTitle("Voice Channel Leave")
                 .WithColor(Color.DarkGrey)
                 .AddField("User", $"<@{user.Id}>", true)
                 .AddField("Channel", before.VoiceChannel.Name, true)
                 .AddField("Time Spent", timeSpent, true);
        }
        // SWITCH
        else if (before.VoiceChannel != after.VoiceChannel)
        {
            embed.WithTitle("Voice Channel Switch")
                 .WithColor(Color.LightGrey)
                 .AddField("User", $"<@{user.Id}>", true)
                 .AddField("From", before.VoiceChannel.Name, true)
                 .AddField("To", after.VoiceChannel.Name, true);
        }
        // MUTE / UNMUTE
        else if (before.IsMuted != after.IsMuted)
        {
            var action = after.IsMuted ? "Muted" : "Unmuted";
            var color = after.IsMuted ? Color.DarkerGrey : Color.LighterGrey;

            embed.WithTitle(action)
                .WithColor(color)
                .AddField("User", $"<@{user.Id}>", true)
                .AddField("Channel", before.VoiceChannel?.Name ?? "Unknown", true);
        }
        // DEAFEN / UNDEAFEN
        else if (before.IsDeafened != after.IsDeafened)
        {
            var action = after.IsDeafened ? "Deafened" : "Undeafened";
            var color = after.IsDeafened ? Color.Orange : Color.LightGrey;

            embed.WithTitle(action)
                .WithColor(color)
                .AddField("User", $"<@{user.Id}>", true)
                .AddField("Channel", before.VoiceChannel?.Name ?? "Unknown", true);
        }
        else if (before.IsSelfMuted != after.IsSelfMuted)
        {
            // TODO: make when muted impact the amount of leveling
        }
        else if (before.IsSelfDeafened != after.IsSelfDeafened)
        {
            // TODO: this should probably just stop leveling entirely
        }
        else
        {
            return;
        }

        await logChannel.SendMessageAsync(embed: embed.Build());
    }
}