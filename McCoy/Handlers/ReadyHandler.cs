﻿using Discord;
using Discord.WebSocket;
using McCoy.Modules;

namespace McCoy.Handlers;

public static class ReadyHandler
{
    public static async Task OnReady(DiscordSocketClient client, Discord.Interactions.InteractionService interaction)
    {
        Console.WriteLine($"Connected as: {client.CurrentUser.Username}#{client.CurrentUser.Discriminator}");
        await client.SetGameAsync("you 🕵️", type: ActivityType.Watching);

        await BotEmbedGenerator.GenerateBotEmbed(client);
        _ = BotEmbedGenerator.StartAutoUpdate(client, 30);
        
        await interaction.RegisterCommandsToGuildAsync(1316537594850443304);
        Console.WriteLine("Commands are registered.");
    }
}