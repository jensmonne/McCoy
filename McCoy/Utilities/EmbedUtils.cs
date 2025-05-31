using System.Globalization;
using Discord.WebSocket;

namespace McCoy.Utilities;

public static class EmbedUtils
{
    public static string FormatTimestamp(DateTimeOffset dt) =>
        dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

    public static string Truncate(this string value, int maxLength) =>
        string.IsNullOrEmpty(value) ? value : 
        value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    
    public static string JumpUrl(SocketTextChannel ch, ulong messageId) =>
        $"https://discord.com/channels/{ch.Guild.Id}/{ch.Id}/{messageId}";
}
