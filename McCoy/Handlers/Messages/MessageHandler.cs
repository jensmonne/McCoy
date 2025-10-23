using Discord.WebSocket;
using McCoy.Core;
using McCoy.Features.Messages;
using McCoy.Modules.Config;

namespace McCoy.Handlers.Messages;

public static class MessageHandler
{
    public static async Task HandleMessage(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        var content = message.Content.Trim().ToLower();

        bool isPregnantUser = false;
        if (message.Channel is SocketGuildChannel guildChannel)
        {
            isPregnantUser = UserConfigService.IsUserInGroup(guildChannel.Guild.Id, UserTypes.Pregnant, message.Author.Id);
        }

        bool isGay = content.Contains("gay");
        if (isPregnantUser || isGay)
        {
            Impregnate.Impregnation(message, isGay);
        }

        switch (content)
        {
            case "hello there":
                await message.Channel.SendMessageAsync("General Kenobi");
                break;
        }
    }
}