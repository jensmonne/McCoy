using Discord;
using Discord.WebSocket;

namespace McCoy.Features.Messages;

public class Impregnate
{
    private static readonly Random random = new Random();

    public static void Impregnation(SocketMessage message, bool isGay)
    {
        // Gary it already checks if the word "gay" is said or if it is chloe who says the message (:
        var malePregnancy = new Emoji("🫃");
        
        if (isGay)
        {
            message.AddReactionAsync(malePregnancy);
            return;
        }
        
        if (random.Next(1, 12) == 3)
        {
            message.AddReactionAsync(malePregnancy);
        }
    }
}