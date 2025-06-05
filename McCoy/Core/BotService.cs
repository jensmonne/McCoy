using Discord.Interactions;
using Discord.WebSocket;
using McCoy.Handlers;
using McCoy.Handlers.Core;
using McCoy.Handlers.Messages;
using McCoy.Handlers.Voice;
using Microsoft.Extensions.DependencyInjection;

namespace McCoy.Core;

public class BotService
{
    public readonly DiscordSocketClient Client;
    public readonly InteractionService Interaction;
    public readonly IServiceProvider Services;

    public BotService(DiscordSocketClient client, InteractionService interaction)
    {
        Client = client;
        Interaction = interaction;

        Services = new ServiceCollection()
            .AddSingleton(Client)
            .AddSingleton(Interaction)
            .BuildServiceProvider();

        InteractionHandler.Initialize(Client, Interaction, Services);
    }

    public void ConfigureEventHandlers()
    {
        Client.Log += LogHandler.HandleLog;
        Interaction.Log += LogHandler.HandleLog;
        Client.Ready += () => ReadyHandler.OnReady(Client, Interaction);
        Client.InteractionCreated += InteractionHandler.HandleInteraction;
        Client.MessageReceived += MessageHandler.HandleMessage;
        Client.MessageDeleted += MessageDeletedHandler.OnMessageDeleted;
        Client.MessageUpdated += MessageUpdatedHandler.OnMessageUpdated;
        Client.UserVoiceStateUpdated += VoiceHandler.OnUserVoiceStateUpdated;
    }
}