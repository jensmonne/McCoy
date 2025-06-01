using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using McCoy.Modules;

namespace McCoy.Commands;

public class ConfigCommands : InteractionModuleBase<SocketInteractionContext>
{
    // ====== CHANNEL COMMANDS ======
    
    [SlashCommand("setchannel", "Set a log or utility channel for this server.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SetChannel(ChannelTypes type, SocketTextChannel channel)
    {
        ChannelConfigService.SetChannel(Context.Guild.Id, type, channel.Id);
        await RespondAsync($"{type} channel set to {channel.Mention}.");
    }

    [SlashCommand("clearchannel", "Clear a configured channel.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ClearChannel(ChannelTypes type)
    {
        bool removed = ChannelConfigService.RemoveChannel(Context.Guild.Id, type);
        await RespondAsync(removed
            ? $"{type} channel has been cleared."
            : $"{type} channel was not set.");
    }

    [SlashCommand("showchannels", "List all configured channels.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ShowChannels()
    {
        var guildId = Context.Guild.Id;
        var embed = new EmbedBuilder()
            .WithTitle("Configured Channels")
            .WithColor(Color.Blue);

        foreach (ChannelTypes type in Enum.GetValues(typeof(ChannelTypes)))
        {
            var channelId = ChannelConfigService.GetChannel(guildId, type);
            if (channelId.HasValue)
                embed.AddField(type.ToString(), $"<#{channelId}>", inline: true);
        }

        await RespondAsync(embed: embed.Build());
    }
    
    // ====== USER GROUP COMMANDS ======
    
    [SlashCommand("addusertogroup", "Add a user to a group.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task AddUserToGroup(UserTypes group, SocketUser user)
    {
        UserConfigService.AddUserToGroup(Context.Guild.Id, group, user.Id);
        await RespondAsync($"{user.Mention} added to group `{group}`.");
    }

    [SlashCommand("removeuserfromgroup", "Remove a user from a group.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task RemoveUserFromGroup(UserTypes group, SocketUser user)
    {
        UserConfigService.RemoveUserFromGroup(Context.Guild.Id, group, user.Id);
        await RespondAsync($"{user.Mention} removed from group `{group}`.");
    }

    [SlashCommand("listgroupusers", "List all users in a group.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ListUsersInGroup(UserTypes group)
    {
        var userIds = UserConfigService.GetUsersInGroup(Context.Guild.Id, group);

        if (userIds.Count == 0)
        {
            await RespondAsync($"Group `{group}` is empty or does not exist.");
            return;
        }

        var userMentions = userIds.Select(id =>
        {
            var user = Context.Guild.GetUser(id);
            return user != null ? user.Mention : $"`Unknown user ({id})`";
        });

        await RespondAsync($"Users in `{group}`:\n{string.Join("\n", userMentions)}");
    }
}