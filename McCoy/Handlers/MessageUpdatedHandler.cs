using Discord;
using Discord.WebSocket;

namespace McCoy.Handlers;

public class MessageUpdatedHandler
{
    private static readonly ulong LogChannelId = 1378141651301695538;

    public static async Task OnMessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        if (after.Author.IsBot || string.IsNullOrWhiteSpace(after.Content)) return;
        
        var beforeMsg = await before.GetOrDownloadAsync();
        if (beforeMsg == null || beforeMsg.Content == after.Content) return;
        
        if (channel is not SocketTextChannel textChannel) return;
        
        var logChannel = textChannel.Guild.GetTextChannel(LogChannelId);
        if (logChannel == null) return;
        
        var embed = new EmbedBuilder()
            .WithTitle("Message Edited")
            .AddField("Author", after.Author, true)
            .AddField("Channel", textChannel.Mention, true)
            .AddField("Before", string.IsNullOrWhiteSpace(beforeMsg.Content) ? "*[no text]*" : beforeMsg.Content)
            .AddField("After", string.IsNullOrWhiteSpace(after.Content) ? "*[no text]*" : after.Content)
            .WithTimestamp(DateTimeOffset.Now)
            .WithColor(Color.Orange)
            .Build();

        await logChannel.SendMessageAsync(embed: embed);
    }
}