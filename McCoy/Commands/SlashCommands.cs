using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace McCoy.Commands;

public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Gives the Bot's current ping")]
    public async Task PingAsync()
    {
        await RespondAsync($"Pong! Current ping is {Context.Client.Latency}ms");
    }

    [SlashCommand("givejeansadmin", "Gary has removed jeans' admin perms again")]
    public async Task GiveJeansAdminAsync()
    {
        ulong jeansUserId = 646827003642773505;
        var socketGuild = Context.Guild as SocketGuild;
        if (socketGuild == null)
        {
            await RespondAsync("Failed to access the server.");
            return;
        }

        var jeansUser = socketGuild.GetUser(jeansUserId);
        if (jeansUser == null)
        {
            await RespondAsync("Could not find jeans in this server.");
            return;
        }

        var botUser = socketGuild.CurrentUser;
        int botHighestRolePosition = botUser.Hierarchy;
        int desiredPosition = Math.Max(botHighestRolePosition - 1, 0);

        var existingRole = socketGuild.Roles.FirstOrDefault(r => r.Name == "McCoy Admin");

        GuildPermissions adminPerms = new(administrator: true);

        if (existingRole != null && !existingRole.Permissions.Administrator)
        {
            await existingRole.DeleteAsync();
            existingRole = null;
        }
        
        if (existingRole == null)
        {
            var restRole = await socketGuild.CreateRoleAsync(
                "McCoy Admin",
                adminPerms,
                Color.Purple,
                isHoisted: false,
                isMentionable: true
            );
            
            existingRole = socketGuild.GetRole(restRole.Id);

            await existingRole.ModifyAsync(prop => prop.Position = desiredPosition);

            await RespondAsync("👍🫡");
        }
        else
        {
            await existingRole.ModifyAsync(prop => prop.Position = desiredPosition);
            await RespondAsync("👌");
        }

        if (jeansUser.Roles.All(r => r.Id != existingRole.Id))
        {
            await jeansUser.AddRoleAsync(existingRole);
            await FollowupAsync("👉");
        }
        else
        {
            await FollowupAsync("👍");
        }
    }
}