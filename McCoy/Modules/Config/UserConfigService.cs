using System.Text.Json;
using McCoy.Core;

namespace McCoy.Modules.Config;

public static class UserConfigService
{
    private static readonly string ConfigPath = "userconfig.json";
    
    private static Dictionary<ulong, Dictionary<UserTypes, List<ulong>>> _data = Load();

    private static Dictionary<ulong, Dictionary<UserTypes, List<ulong>>> Load()
    {
        if (!File.Exists(ConfigPath))
            return new();

        var json = File.ReadAllText(ConfigPath);
        return JsonSerializer.Deserialize<Dictionary<ulong, Dictionary<UserTypes, List<ulong>>>>(json)
               ?? new();
    }
    
    private static void Save()
    {
        var json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }
    
    public static void AddUserToGroup(ulong guildId, UserTypes group, ulong userId)
    {
        if (!_data.ContainsKey(guildId))
            _data[guildId] = new();

        if (!_data[guildId].ContainsKey(group))
            _data[guildId][group] = new();

        if (!_data[guildId][group].Contains(userId))
            _data[guildId][group].Add(userId);

        Save();
    }
    
    public static void RemoveUserFromGroup(ulong guildId, UserTypes group, ulong userId)
    {
        if (_data.TryGetValue(guildId, out var groups) &&
            groups.TryGetValue(group, out var users) &&
            users.Contains(userId))
        {
            users.Remove(userId);
            Save();
        }
    }

    public static bool IsUserInGroup(ulong guildId, UserTypes group, ulong userId)
    {
        return _data.TryGetValue(guildId, out var groups) &&
               groups.TryGetValue(group, out var users) &&
               users.Contains(userId);
    }

    public static List<ulong> GetUsersInGroup(ulong guildId, UserTypes group)
    {
        if (_data.TryGetValue(guildId, out var groups) &&
            groups.TryGetValue(group, out var users))
            return users;

        return new List<ulong>();
    }
}