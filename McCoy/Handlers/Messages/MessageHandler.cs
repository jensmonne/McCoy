using Discord.WebSocket;
using McCoy.Core;
using McCoy.Features.Messages;
using McCoy.Modules;
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
            case "hi":
                await message.Channel.SendMessageAsync(
                    "My name is Walter Hartwell White. " +
                    "I live at 308 Negra Arroyo Lane, Albuquerque, New Mexico, 87104. " +
                    "To all law enforcement entities, this is not an admission of guilt. " +
                    "I am speaking to my family now. Skyler, you are the love of my life. " +
                    "I hope you know that. Walter Jr., you're my big man. " +
                    "There are going to be some things that you'll come to learn about me in the next few days. " +
                    "But just know that no matter how it may look, I only had you in my heart. Goodbye.");
                break;
            case "hello there":
                await message.Channel.SendMessageAsync("General Kenobi");
                break;
        }
    }
}