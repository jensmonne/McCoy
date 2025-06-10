namespace McCoy.Utilities;

public static class MessageUtils
{
    private static readonly Random random = new Random();
    
    public static string GetRandomEmote()
    {
        string[] emotes = { "👋", "🫡", "🥳", "🎉", "🤠" };
        int index = random.Next(emotes.Length);
        return emotes[index];
    }
}