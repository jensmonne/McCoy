using Discord;
using Discord.WebSocket;
using McCoy.Modules;
using McCoy.Modules.Config;
using McCoy.Modules.Embeds;

namespace McCoy.Handlers.Core;

public static class ReadyHandler
{
    public static async Task OnReady(DiscordSocketClient client, Discord.Interactions.InteractionService interaction)
    {
        Console.WriteLine($"Connected as: {client.CurrentUser.Username}#{client.CurrentUser.Discriminator}");
        await client.SetGameAsync("you 🕵️", type: ActivityType.Watching);

        await BotEmbedGenerator.GenerateBotEmbed(client);
        _ = BotEmbedGenerator.StartAutoUpdate(client, 30);
        
        foreach (var guildId in ConfigService.DevGuilds)
        {
            await interaction.RegisterCommandsToGuildAsync(guildId, true);
        }
        
        Console.WriteLine("Commands are registered.");
    }
}