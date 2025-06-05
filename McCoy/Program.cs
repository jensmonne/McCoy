﻿using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DotNetEnv;
using McCoy.Core;

namespace McCoy;

public class Program
{
    public static async Task Main(string[] args)
    {
        Env.Load();

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
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