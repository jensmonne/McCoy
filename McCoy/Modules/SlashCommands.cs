using Discord.Interactions;

namespace McCoy.Modules;

public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Gives the Bot's current ping")]
    public async Task PingAsync()
    {
        await RespondAsync($"Pong! Current ping is {Context.Client.Latency}ms");
    }
}