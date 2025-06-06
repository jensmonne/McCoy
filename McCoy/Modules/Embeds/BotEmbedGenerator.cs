using Discord;
using Discord.WebSocket;
using McCoy.Core;
using McCoy.Modules.Config;
using McCoy.Utilities;

namespace McCoy.Modules.Embeds;

// This stuff will probably be handled by a seperate watchdog discord bot

public static class BotEmbedGenerator
{
    private static readonly DateTime _startTime = DateTime.UtcNow;
    private static IUserMessage? _statusMessage;
    private const string StatusMessagePath = "config/statusmsg.txt";

    public static async Task GenerateBotEmbed(DiscordSocketClient client)
    {
        var guild = client.Guilds.FirstOrDefault();
        if (guild == null)
        {
            Console.WriteLine("No guilds found.");
            return;
        }

        var channelId = ChannelConfigService.GetChannel(guild.Id, ChannelTypes.Status);
        if (channelId == null)
        {
            Console.WriteLine("No status channel configured.");
            return;
        }

        if (guild.GetTextChannel(channelId.Value) is not IMessageChannel channel)
        {
            Console.WriteLine("Configured status channel not found.");
            return;
        }

        var uptime = DateTime.UtcNow - _startTime;

        var embed = new EmbedBuilder()
            .WithTitle("McCoy is Online")
            .WithColor(Color.Green)
            .AddField("Status", client.Status.ToString(), true)
            .AddField("Uptime", EmbedUtils.FormatDuration(uptime), true)
            .AddField("Ping", $"{client.Latency} ms", true)
            .AddField("Version", ConfigService.BotVersion, true)
            .AddField("Last updated on", $"<t:{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}:f>")
            .Build();
        
        if (_statusMessage == null)
        {
            ulong? messageId = LoadMessageId();
            if (messageId.HasValue)
            {
                try
                {
                    var message = await channel.GetMessageAsync(messageId.Value);
                    if (message is IUserMessage userMsg) _statusMessage = userMsg;
                }
                catch
                {
                    Console.WriteLine("Could not retrieve saved status message.");
                }
            }
        }

        if (_statusMessage == null)
        {
            _statusMessage = await channel.SendMessageAsync(embed: embed);
            SaveMessageId(_statusMessage.Id);
        }
        else
        {
            try
            {
                await _statusMessage.ModifyAsync(msg => msg.Embed = embed);
            }
            catch
            {
                Console.WriteLine("Status message invalid. Creating a new one.");
                _statusMessage = await channel.SendMessageAsync(embed: embed);
                SaveMessageId(_statusMessage.Id);
            }
        }
    }

    public static async Task StartAutoUpdate(DiscordSocketClient client, int intervalSeconds = 60)
    {
        while (true)
        {
            try
            {
                await GenerateBotEmbed(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating status embed: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds));
        }
    }

    private static void SaveMessageId(ulong id)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(StatusMessagePath)!);
        File.WriteAllText(StatusMessagePath, id.ToString());
    }

    private static ulong? LoadMessageId()
    {
        if (!File.Exists(StatusMessagePath))
            return null;

        var text = File.ReadAllText(StatusMessagePath);
        return ulong.TryParse(text, out var id) ? id : null;
    }
}
