using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using McCoy.Core;

namespace McCoy;

public class Program
{
    public static async Task Main(string[] args)
    {
        DotNetEnv.Env.Load();

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                var config = new DiscordSocketConfig
                {
                    MessageCacheSize = 1000,
                    GatewayIntents = GatewayIntents.Guilds |
                                     GatewayIntents.GuildMessages |
                                     GatewayIntents.MessageContent |
                                     GatewayIntents.GuildVoiceStates |
                                     GatewayIntents.GuildMembers
                };
                
                services.AddSingleton(config);
                services.AddSingleton<DiscordSocketClient>();
                services.AddSingleton(provider =>
                {
                    var client = provider.GetRequiredService<DiscordSocketClient>();
                    return new InteractionService(client.Rest);
                });
                services.AddSingleton<BotService>();
                services.AddHostedService<BotHostedService>();
            })
            .Build();

        await host.RunAsync();
    }
}