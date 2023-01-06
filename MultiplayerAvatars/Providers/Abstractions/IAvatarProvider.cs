using CustomAvatar.Avatar;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerAvatars.Providers.Abstractions
{
    public interface IAvatarProvider
    {
        Task<AvatarPrefab?> GetAvatarByHash(string hash, CancellationToken cancellationToken);
    }
}
