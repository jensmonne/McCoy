using Discord.WebSocket;
using McCoy.Core;
using McCoy.Modules.Config;

namespace McCoy.Features.Voices;

public class ClaimableVC
{
    private static readonly Dictionary<ulong, ulong?> vcOwners = new();
    
    public static async Task VCClaimableJoin(SocketVoiceChannel vc, SocketUser user)
    {
        var channelId = ChannelConfigService.GetChannel(vc.Guild.Id, ChannelTypes.ClaimableVc);
        if (channelId == null || vc.Id != channelId) return;
        
        if (vc.ConnectedUsers.Count == 1)
        {
            vcOwners[vc.Id] = user.Id;

            if (vc.Name != $"{user.Username}'s VC")
            {
                await vc.ModifyAsync(props => { props.Name = $"{user.Username}'s VC"; });
            }
        }
    }

    public static async Task VCClaimableLeave(SocketVoiceChannel vc, SocketUser user)
    {
        var channelId = ChannelConfigService.GetChannel(vc.Guild.Id, ChannelTypes.ClaimableVc);
        if (channelId == null || vc.Id != channelId) return;
        
        if (vc.ConnectedUsers.Count == 0) 
        {
            if (vc.Name != "Claimable VC" || vc.UserLimit != 32)
            {
                await vc.ModifyAsync(props =>
                {
                    props.Name = "Claimable VC";
                    props.UserLimit = 32;
                });
            }

            vcOwners.Remove(vc.Id);
        }
        else if (vc.ConnectedUsers.Count >= 1)
        {
            var nextUser = vc.ConnectedUsers.FirstOrDefault();
            if (nextUser == null) return;
            
            vcOwners[vc.Id] = nextUser.Id;
            await vc.ModifyAsync(props => { props.Name = $"{nextUser.Username}'s VC"; });
        }
    }

    public static async Task VCClaimableSwitch(SocketVoiceChannel vc, SocketUser user, bool isJoining)
    {
        var channelId = ChannelConfigService.GetChannel(vc.Guild.Id, ChannelTypes.ClaimableVc);
        if (channelId == null || vc.Id != channelId) return;

        if (isJoining)
        {
            if (!vcOwners.ContainsKey(vc.Id) || vcOwners[vc.Id] == null)
            {
                vcOwners[vc.Id] = user.Id;
                await vc.ModifyAsync(props => { props.Name = $"{user.Username}'s VC"; });
            }
        }
        else
        {
            if (vcOwners.TryGetValue(vc.Id, out var ownerId) && ownerId == user.Id)
            {
                var remainingUsers = vc.ConnectedUsers.Where(u => u.Id != user.Id).ToList();

                if (remainingUsers.Count == 0)
                {
                    await vc.ModifyAsync(props =>
                    {
                        props.Name = "Claimable VC";
                        props.UserLimit = 32;
                    });
                    vcOwners.Remove(vc.Id);
                }
                else
                {
                    var nextUser = remainingUsers.First();
                    vcOwners[vc.Id] = nextUser.Id;
                    await vc.ModifyAsync(props => { props.Name = $"{nextUser.Username}'s VC"; });
                }
            }
        }
    }
}