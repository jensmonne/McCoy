using Discord;
using Discord.WebSocket;
using DotNetEnv;

class Program
{
    private static DiscordSocketClient? _client;

    public static async Task Main()
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
        };
        
        _client = new DiscordSocketClient(config);
        
        _client.Log += Log;
        _client.MessageReceived += OnMessageReceived;
        
        Env.Load();

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

    private static async Task OnMessageReceived(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        var allowUpper = message.Content.ToLower();
        

        if (allowUpper == "!fart")
        {
            await message.Channel.SendMessageAsync("FART!");
        }

        if (allowUpper == "hi")
        {
            await message.Channel.SendMessageAsync(
                "My name is Walter Hartwell White. I live at 308 Negra Arroyo Lane, Albuquerque, New Mexico, 87104. To all law enforcement entities, this is not an admission of guilt. I am speaking to my family now. Skyler, you are the love of my life. I hope you know that. Walter Jr., you're my big man. There are going to be some things that you'll come to learn about me in the next few days. But just know that no matter how it may look, I only had you in my heart. Goodbye.");
        }

        if (allowUpper == "ping")
        {
            await message.Channel.SendMessageAsync("Pong!");
        }
        
    }
}