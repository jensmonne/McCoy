using System.Text.Json;
using System.Text.Json.Serialization;
using McCoy.Core;

namespace McCoy.Modules.Config;

public static class ChannelConfigService
{
    private static readonly string ConfigPath = "channelconfig.json";

    private static Dictionary<ulong, Dictionary<ChannelTypes, ulong>> _config = Load();

    private static Dictionary<ulong, Dictionary<ChannelTypes, ulong>> Load()
    {
        if (!File.Exists(ConfigPath))
            return new();

        var json = File.ReadAllText(ConfigPath);
        return JsonSerializer.Deserialize<Dictionary<ulong, Dictionary<ChannelTypes, ulong>>>(json)
               ?? new();
    }

    private static void Save()
    {
        var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        });
        File.WriteAllText(ConfigPath, json);
    }

    public static void SetChannel(ulong guildId, ChannelTypes type, ulong channelId)
    {
        if (!_config.ContainsKey(guildId))
            _config[guildId] = new();

        _config[guildId][type] = channelId;
        Save();
    }

    public static ulong? GetChannel(ulong guildId, ChannelTypes type)
    {
        return _config.TryGetValue(guildId, out var map) && map.TryGetValue(type, out var id)
            ? id
            : null;
    }

    public static bool RemoveChannel(ulong guildId, ChannelTypes type)
    {
        if (_config.TryGetValue(guildId, out var map) && map.Remove(type))
        {
            Save();
            return true;
        }

        return false;
    }
}