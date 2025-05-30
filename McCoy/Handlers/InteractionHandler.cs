using Discord.Interactions;
using Discord.WebSocket;

namespace McCoy.Handlers;

public class InteractionHandler
{
    private static DiscordSocketClient _client;
    private static InteractionService _commands;
    private static IServiceProvider _services;
    
    public static void Initialize(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
    {
        _client = client;
        _commands = commands;
        _services = services;
    }
    
    public static async Task HandleInteraction(SocketInteraction interaction)
    {
        var ctx = new SocketInteractionContext(_client, interaction);
        await _commands.ExecuteCommandAsync(ctx, _services);
    }
}