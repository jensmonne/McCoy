﻿using Discord;
using Discord.WebSocket;

namespace McCoy.Handlers.Messages;

public class Impregnate
{
    public static void Impregnation(SocketMessage message, bool isGay)
    {
        // Gary it already checks if the word "gay" is said or if it is chloe who says the message (:
        var MalePregnancy = new Emoji("🫃");
        message.AddReactionAsync(MalePregnancy);
        
        if (message, bool isGay)
        {
            message.AddReactionAsync(MalePregnancy);
        }
    }
}