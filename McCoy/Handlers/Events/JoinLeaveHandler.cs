using Discord.WebSocket;
using McCoy.Core;
using McCoy.Modules.Config;
using McCoy.Utilities;

namespace McCoy.Handlers.Events;

public static class JoinLeaveHandler
{
    public static async Task OnUserJoin(SocketGuildUser user)
    {
        var channelId = ChannelConfigService.GetChannel(user.Guild.Id, ChannelTypes.Welcome);
        if (channelId is not ulong joinChannelId) return;

        var joinChannel = user.Guild.GetTextChannel(joinChannelId);
        if (joinChannel == null) return;
        
        await joinChannel.SendMessageAsync($"{MessageUtils.GetRandomEmote()} Welcome to CastleCraft {user.Mention}, be sure to check out the reaction roles at the top of the channel list!");
    }

    public static async Task OnUserLeave(SocketGuild guild, SocketUser user)
    {
        var channelId = ChannelConfigService.GetChannel(guild.Id, ChannelTypes.Goodbye);
        if (channelId is not ulong leaveChannelId) return;

        var leaveChannel = guild.GetTextChannel(leaveChannelId);
        if (leaveChannel == null) return;
        
        await leaveChannel.SendMessageAsync($"{user.Mention} has left CastleCraft D:<");
    }
}