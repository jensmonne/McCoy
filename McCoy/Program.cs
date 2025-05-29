using Discord;
using Discord.WebSocket;
using DotNetEnv;
using McCoy.Handlers;
class Program
{
    private static DiscordSocketClient? _client;

    public static async Task Main()
    {
        Env.Load();
        
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
        };
        
        _client = new DiscordSocketClient(config);
        
        _client.Log += Log;
        _client.MessageReceived += MessageHandler.HandleMessage;
        

        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")
            ?? throw new InvalidOperationException("DISCORD_TOKEN is not set.");
        
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        
        await Task.Delay(-1);
    }

    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}