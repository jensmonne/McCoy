using Discord;
using Discord.WebSocket;
using McCoy.Core;
using McCoy.Features.Voices;
using McCoy.Modules.Config;
using McCoy.Utilities;

namespace McCoy.Handlers.Voice;

public static class VoiceHandler
{
    private static readonly Dictionary<ulong, DateTime> VoiceJoinTimes = new();

    public static async Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        if (user.IsBot) return;

        var guildUser = user as SocketGuildUser;
        var guild = guildUser?.Guild;
        if (guild == null) return;
        
        var channelId = ChannelConfigService.GetChannel(guild.Id, ChannelTypes.VoiceLogs);
        if (channelId is not ulong logChannelId) return;

        var logChannel = guild.GetTextChannel(logChannelId);
        if (logChannel == null) return;
        
        var now = DateTime.UtcNow;
        var garyNow = EmbedUtils.GetAmsterdamTime();

        var embed = new EmbedBuilder().WithAuthor(user);

        // JOIN
        if (before.VoiceChannel == null && after.VoiceChannel != null)
        {
            ClaimableVC.VCClaimableJoin(after.VoiceChannel, user);
            
            VoiceJoinTimes[user.Id] = now;

            embed.WithTitle("Member Joined Voice Channel")
                .WithColor(Color.Green)
                .AddField("User", $"<@{user.Id}>", true)
                .AddField("User ID", user.Id, true)
                .AddField("Channel", after.VoiceChannel.Mention, true)
                .AddField("Channel ID", after.VoiceChannel.Id, true)
                .AddField("Time", $"<t:{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}:f>", true)
                .WithFooter($"Gary time: {garyNow}");
            
            var members = after.VoiceChannel.ConnectedUsers.Select(u => $"<@{u.Id}>");
            embed.AddField("Users in Channel", string.Join(", ", members));
        }
        // LEAVE
        else if (before.VoiceChannel != null && after.VoiceChannel == null)
        {
            ClaimableVC.VCClaimableLeave(before.VoiceChannel, user);
            
            var joinTime = VoiceJoinTimes.ContainsKey(user.Id) ? VoiceJoinTimes[user.Id] : (DateTime?)null;
            VoiceJoinTimes.Remove(user.Id);

            var timeSpent = joinTime.HasValue
                ? EmbedUtils.FormatDuration(now - joinTime.Value)
                : "unknown";

            embed.WithTitle("Member Left Voice Channel")
                 .WithColor(Color.Red)
                 .AddField("User", $"<@{user.Id}>", true)
                 .AddField("User ID", user.Id, true)
                 .AddField("Channel", before.VoiceChannel.Mention, true)
                 .AddField("Channel ID", before.VoiceChannel.Id, true)
                 .AddField("Time", $"<t:{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}:f>", true)
                 .AddField("Time Spent", timeSpent, true)
                 .WithFooter($"Gary time: {garyNow}");
            
            var memberList = before.VoiceChannel.ConnectedUsers?.Select(u => $"<@{u.Id}>").ToList();

            var userListText = memberList != null && memberList.Any() ? string.Join(", ", memberList) : "No users remaining.";
            embed.AddField("Users in Channel", userListText);
        }
        // SWITCH
        else if (before.VoiceChannel != after.VoiceChannel)
        {
            var vc1 = ChannelConfigService.GetChannel(before.VoiceChannel.Guild.Id, ChannelTypes.ClaimableVc);
            if (vc1 != null && before.VoiceChannel.Id == vc1)
            {
                await ClaimableVC.VCClaimableSwitch(before.VoiceChannel, user, false);
            }
            var vc2 = ChannelConfigService.GetChannel(after.VoiceChannel.Guild.Id, ChannelTypes.ClaimableVc);
            if (vc2 != null && after.VoiceChannel.Id == vc2)
            {
                await ClaimableVC.VCClaimableSwitch(after.VoiceChannel, user, true);
            }
            
            var joinTime = VoiceJoinTimes.TryGetValue(user.Id, out var time) ? time : (DateTime?)null;
            VoiceJoinTimes[user.Id] = now;
            
            var timeSpent = joinTime.HasValue
                ? EmbedUtils.FormatDuration(now - joinTime.Value)
                : "unknown";
            
            embed.WithTitle("Member Switched Voice Channel")
                 .WithColor(Color.Orange)
                 .AddField("User", $"<@{user.Id}>", true)
                 .AddField("User ID", user.Id, true)
                 .AddField("From", before.VoiceChannel.Mention, true)
                 .AddField("To", after.VoiceChannel.Mention, true)
                 .AddField("Time", $"<t:{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}:f>", true)
                 .AddField("Time Spent", timeSpent, true)
                 .WithFooter($"Gary time: {garyNow}");
            
            var memberList = before.VoiceChannel.ConnectedUsers?.Select(u => $"<@{u.Id}>").ToList();

            var userListText = memberList?.Any() == true ? string.Join(", ", memberList) : "No users remaining.";
            embed.AddField($"Users in {before.VoiceChannel.Mention}", userListText);
            
            var aftermemberList = after.VoiceChannel.ConnectedUsers?.Select(u => $"<@{u.Id}>").ToList();

            var afteruserListText = aftermemberList != null && aftermemberList.Any() ? string.Join(", ", aftermemberList) : "No users remaining.";
            embed.AddField($"Users in {after.VoiceChannel.Mention}", afteruserListText);
        }
        // MUTE / UNMUTE
        else if (before.IsMuted != after.IsMuted)
        {
            var action = after.IsMuted ? "Muted" : "Unmuted";
            var color = after.IsMuted ? Color.Red : Color.Green;

            embed.WithTitle(action)
                .WithColor(color)
                .AddField("User", $"<@{user.Id}>", true)
                .AddField("Channel", before.VoiceChannel?.Name ?? "Unknown", true);
        }
        // DEAFEN / UNDEAFEN
        else if (before.IsDeafened != after.IsDeafened)
        {
            var action = after.IsDeafened ? "Deafened" : "Undeafened";
            var color = after.IsDeafened ? Color.Red : Color.Green;

            embed.WithTitle(action)
                .WithColor(color)
                .AddField("User", $"<@{user.Id}>", true)
                .AddField("Channel", before.VoiceChannel?.Name ?? "Unknown", true);
        }
        else if (before.IsSelfMuted != after.IsSelfMuted)
        {
            // TODO: make when muted impact the amount of leveling
            return;
        }
        else if (before.IsSelfDeafened != after.IsSelfDeafened)
        {
            // TODO: this should probably just stop leveling entirely
            return;
        }
        else
        {
            return;
        }

        await logChannel.SendMessageAsync(embed: embed.Build());
    }
}