using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DotNetEnv;
using McCoy.Handlers;

class Bot
{
    private static DiscordSocketClient? _client;
    private static InteractionService? _interaction;

    public static async Task Main()
    {
        Env.Load();
        
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.Guilds | GatewayIntents.GuildMessages,
        };
        
        _client = new DiscordSocketClient(config);
        _interaction = new InteractionService(_client.Rest);
        
        _client.Log += LogHandler.HandleMessage;
        _interaction.Log += LogHandler.HandleMessage;
        _client.Ready += () => ReadyHandler.OnReady(_client);
        _client.MessageReceived += MessageHandler.HandleMessage;

        string token = Env.GetString("DISCORD_TOKEN");
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        
        await Task.Delay(-1);
    }
}