﻿using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DotNetEnv;
using McCoy.Handlers;

class Bot
{
    private static DiscordSocketClient? _client;
    private static InteractionService? _interaction;
    private static IServiceProvider _services;

    public static async Task Main()
    {
        Env.Load();
        
        var config = new DiscordSocketConfig
        {
            MessageCacheSize = 1000,
            GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
        };
        
        _client = new DiscordSocketClient(config);
        _interaction = new InteractionService(_client.Rest);
        InteractionHandler.Initialize(_client, _interaction, _services);
        
        _client.Log += LogHandler.HandleLog;
        _interaction.Log += LogHandler.HandleLog;
        _client.Ready += () => ReadyHandler.OnReady(_client, _interaction);
        _client.InteractionCreated += InteractionHandler.HandleInteraction;
        _client.MessageReceived += MessageHandler.HandleMessage;
        _client.MessageDeleted += MessageDeletedHandler.OnMessageDeleted;
        _client.MessageUpdated += MessageUpdatedHandler.OnMessageUpdated;

        string token = Env.GetString("DISCORD_TOKEN");
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        
        await _interaction.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("Shutting down...");
            e.Cancel = true;
            cts.Cancel();
        };
        
        AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
        {
            Console.WriteLine("Process exiting...");
            cts.Cancel();
        };
        
        try
        {
            await Task.Delay(-1, cts.Token);
        }
        catch (TaskCanceledException)
        {
            // Expected on shutdown
        }
        finally
        {
            await _client.StopAsync();
            await _client.LogoutAsync();
            _client.Dispose();
        }
    }
}