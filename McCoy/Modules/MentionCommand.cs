using Discord;
using Discord.Interactions;

namespace McCoy.Modules;

public class MentionCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("mention", "mentions users in categories")]
    public async Task Mention()
    {
        var builder = new ComponentBuilder().WithSelectMenu(new SelectMenuBuilder()
            .WithCustomId("mention_select")
            .WithPlaceholder("Choose a category to mention")
            .AddOption("Lethal Company", "lc")
        );
        
        await RespondAsync("Choose a category to mention", components: builder.Build());
    }
}