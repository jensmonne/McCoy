using Discord;
using Discord.WebSocket;
using McCoy.Modules.Config;
using McCoy.Modules.Embeds;

namespace McCoy.Handlers.Core;

public static class ReadyHandler
{
    public static Task OnReady(DiscordSocketClient client, Discord.Interactions.InteractionService interaction)
    {
        _ = Task.Run(async () =>
        {
            Console.WriteLine($"Connected as: {client.CurrentUser.Username}#{client.CurrentUser.Discriminator}");
            await client.SetGameAsync("you 🕵️", type: ActivityType.Watching);

            await BotEmbedGenerator.GenerateBotEmbed(client);
            _ = BotEmbedGenerator.StartAutoUpdate(client, 30);

            /*foreach (var guildId in ConfigService.DevGuilds)
            {
                var guild = client.GetGuild(guildId);
                if (guild == null) continue;

                var existingCommands = await guild.GetApplicationCommandsAsync();
                foreach (var cmd in existingCommands)
                {
                    await cmd.DeleteAsync();
                }
                
                await interaction.RegisterCommandsToGuildAsync(guildId, true);
            }*/
            
            foreach (var guildId in ConfigService.DevGuilds)
            {
                await interaction.RegisterCommandsToGuildAsync(guildId, true);
            }

            Console.WriteLine("Commands are registered.");
        });

        return Task.CompletedTask;
    }
}