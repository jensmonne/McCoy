using Discord;
using Discord.WebSocket;

namespace McCoy.Handlers;

public static class MessageDeletedHandler
{
    private static readonly ulong LogChannelId = 1378141651301695538;

    public static async Task OnMessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> channel)
    {
        var msg = await cachedMessage.GetOrDownloadAsync();
        var ch = await channel.GetOrDownloadAsync();
        
        if (string.IsNullOrWhiteSpace(msg.Content) || msg.Author.IsBot)
            return;
        
        if (ch is not SocketTextChannel textChannel)
            return;
        
        var logChannel = textChannel.Guild.GetTextChannel(LogChannelId);
        if (logChannel == null)
            return;
        
        var embed = new EmbedBuilder()
            .WithTitle("Message Deleted")
            .AddField("Author", msg.Author, true)
            .AddField("Channel", textChannel.Mention, true)
            .AddField("Content", string.IsNullOrWhiteSpace(msg.Content) ? "*[no text]*" : msg.Content)
            .WithTimestamp(DateTimeOffset.Now)
            .WithColor(Color.Red)
            .Build();

        await logChannel.SendMessageAsync(embed: embed);
    }
}