using Discord;
using Discord.WebSocket;

namespace McCoy.Features.Messages;

public class Impregnate
{
    private static readonly Random random = new Random();

    public static void Impregnation(SocketMessage message, bool isGay)
    {
        var malePregnancy = new Emoji("🫃");
        
        if (isGay)
        {
            message.AddReactionAsync(malePregnancy);
            return;
        }
        
        if (random.Next(1, 12) == 1)
        {
            message.AddReactionAsync(malePregnancy);
        }
    }
}