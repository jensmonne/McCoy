using Discord.WebSocket;
using DotNetEnv;
using McCoy.Handlers.Messages;

namespace McCoy.Handlers;

public static class MessageHandler
{
    private static readonly string ChloeId = Env.GetString("CHLOE_ID");
    
    public static async Task HandleMessage(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        var content = message.Content.Trim().ToLower();

        if (message.Author.Id.ToString() == ChloeId || content.Contains("gay"))
        {
            Impregnate.Impregnation(message);
        }

        switch (content)
        {
            case "!fart":
                await message.Channel.SendMessageAsync("FART!");
                break;
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
            case "ping":
                await message.Channel.SendMessageAsync("Pong!");
                break;
            case "hello there":
                await message.Channel.SendMessageAsync("General Kenobi");
                break;
        }
    }
}