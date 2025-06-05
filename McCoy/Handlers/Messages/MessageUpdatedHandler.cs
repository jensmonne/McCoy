﻿using Discord;
using Discord.WebSocket;
using McCoy.Core;
using McCoy.Modules.Config;
using McCoy.Utilities;

namespace McCoy.Handlers.Messages;

public static class MessageUpdatedHandler
{
    public static async Task OnMessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        if (after.Author.IsBot || string.IsNullOrWhiteSpace(after.Content)) return;

        var beforeMsg = await before.GetOrDownloadAsync();
        if (beforeMsg == null || beforeMsg.Content == after.Content) return;
        if (channel is not SocketTextChannel textChannel) return;
        if (after.Author is not SocketGuildUser author) return;

        var channelId = ChannelConfigService.GetChannel(textChannel.Guild.Id, ChannelTypes.EditLogs);
        if (channelId is not ulong logChannelId) return;
        
        var logChannel = textChannel.Guild.GetTextChannel(logChannelId);
        if (logChannel == null) return;

        var joinTimestamp = author.JoinedAt?.ToUnixTimeSeconds();
        
        var embed = new EmbedBuilder()
            .WithTitle("Message Edited")
            .WithColor(Color.Orange)
            .WithDescription($"[Jump to Message]({EmbedUtils.JumpUrl(textChannel, after.Id)})")

            .AddField("Author", $"{author.Mention}\n{author.Username}#{author.Discriminator}\nID: {author.Id}", true)
            .AddField("Author Discord Join Date", author?.CreatedAt != null ? $"<t:{author.CreatedAt.ToUnixTimeSeconds()}:R>" : "Unknown", true)
            .AddField("Author Server Join Date", author?.JoinedAt != null ? $"<t:{joinTimestamp}:R>" : "Unknown", true)
            
            .AddField("Channel", textChannel.Mention, true)
            .AddField("Message Sent At", EmbedUtils.FormatTimestamp(beforeMsg.Timestamp), true)
            .AddField("Edited On", EmbedUtils.FormatTimestamp(DateTimeOffset.UtcNow), true)

            .AddField("Before", string.IsNullOrWhiteSpace(beforeMsg.Content) ? "*[no text]*" : beforeMsg.Content.Truncate(1024))
            .AddField("After", string.IsNullOrWhiteSpace(after.Content) ? "*[no text]*" : after.Content.Truncate(1024))
            
            .Build();

        await logChannel.SendMessageAsync(embed: embed);
    }
    
}