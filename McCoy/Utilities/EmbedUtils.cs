using System.Globalization;
using Discord.WebSocket;

namespace McCoy.Utilities;

public static class EmbedUtils
{
    public static string FormatTimestamp(DateTimeOffset dt) =>
        dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    
    public static string FormatDuration(TimeSpan span)
    {
        var parts = new List<string>();

        if (span.Days > 0)
            parts.Add($"{span.Days}d");
        if (span.Hours > 0)
            parts.Add($"{span.Hours}h");
        if (span.Minutes > 0)
            parts.Add($"{span.Minutes}m");
        if (span.Seconds > 0)
            parts.Add($"{span.Seconds}s");

        return parts.Count > 0 ? string.Join(" ", parts) : "0s";
    }

    public static string Truncate(this string value, int maxLength) =>
        string.IsNullOrEmpty(value) ? value : 
        value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    
    public static string JumpUrl(SocketTextChannel ch, ulong messageId) =>
        $"https://discord.com/channels/{ch.Guild.Id}/{ch.Id}/{messageId}";
}
