using Discord;

namespace McCoy.Handlers.Core;

public static class LogHandler
{
    public static Task HandleLog(LogMessage message)
    {
        Console.ForegroundColor = GetColor(message.Severity);
        Console.WriteLine($"[{message.Severity}] {message.Source}: {message.Message}");
        if (message.Exception is not null)
            Console.WriteLine(message.Exception);
        Console.ResetColor();
        return Task.CompletedTask;
    }
    
    private static ConsoleColor GetColor(LogSeverity severity) => severity switch
    {
        LogSeverity.Critical => ConsoleColor.Red,
        LogSeverity.Error => ConsoleColor.DarkRed,
        LogSeverity.Warning => ConsoleColor.Yellow,
        LogSeverity.Info => ConsoleColor.Gray,
        LogSeverity.Verbose => ConsoleColor.DarkGray,
        LogSeverity.Debug => ConsoleColor.Cyan,
        _ => ConsoleColor.White
    };
}