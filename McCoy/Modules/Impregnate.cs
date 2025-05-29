using Discord;
using Discord.WebSocket;

namespace McCoy.Modules;

public class Impregnate
{
    public static void Impregnation(SocketMessage message)
    {
        // Gary it already checks if the word "gay" is said or if it is chloe who says the message (:
        var MalePregnancy = new Emoji("🫃");
        message.AddReactionAsync(MalePregnancy);
    }
}