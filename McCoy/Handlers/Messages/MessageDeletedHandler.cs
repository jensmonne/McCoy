using Discord;
using Discord.WebSocket;
using McCoy.Core;
using McCoy.Modules.Config;
using McCoy.Utilities;

namespace McCoy.Handlers.Messages;

public static class MessageDeletedHandler
{
    public static async Task OnMessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> channel)
    {
        var msg = await cachedMessage.GetOrDownloadAsync();
        var ch = await channel.GetOrDownloadAsync();
        
        if (msg == null || ch == null) return;
        if (string.IsNullOrWhiteSpace(msg.Content) || msg.Author.IsBot) return;
        if (ch is not SocketTextChannel textChannel) return;
        if (msg.Author is not SocketGuildUser author) return;
        
        var channelId = ChannelConfigService.GetChannel(textChannel.Guild.Id, ChannelTypes.DeleteLogs);
        if (channelId is not ulong logChannelId) return;
        
        var logChannel = textChannel.Guild.GetTextChannel(logChannelId);
        if (logChannel == null) return;
        
        var now = EmbedUtils.GetAmsterdamTime();
        var joinTimestamp = author.JoinedAt?.ToUnixTimeSeconds();
        
        var embed = new EmbedBuilder()
            .WithTitle("Message Deleted")
            .WithColor(Color.Red)
            .WithDescription($"[Jump to Message]({EmbedUtils.JumpUrl(textChannel, msg.Id)})")

            .AddField("Author", $"{msg.Author.Mention}\n{msg.Author.Username}#{msg.Author.Discriminator}\nID: {msg.Author.Id}", true)
            .AddField("Author Discord Join Date", author?.CreatedAt != null ? $"<t:{author.CreatedAt.ToUnixTimeSeconds()}:R>" : "Unknown", true)
            .AddField("Author Server Join Date", author?.JoinedAt != null ? $"<t:{joinTimestamp}:R>" : "Unknown", true)

            .AddField("Channel", textChannel.Mention, true)
            .AddField("Message Sent At", EmbedUtils.FormatTimestamp(msg.Timestamp), true)
            .AddField("Message Deleted At", now, true)

            .AddField("Content", string.IsNullOrWhiteSpace(msg.Content) ? "*[no text]*" : msg.Content.Truncate(1024))
            .Build();

        await logChannel.SendMessageAsync(embed: embed);
    }
}