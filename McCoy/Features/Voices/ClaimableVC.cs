using Discord.WebSocket;
using McCoy.Core;
using McCoy.Modules.Config;

namespace McCoy.Features.Voices;

public class ClaimableVC
{
    private static readonly Dictionary<ulong, ulong> vcOwners = new();
    
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

    public static async Task VCClaimableLeave(SocketVoiceChannel vc)
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
    }
}