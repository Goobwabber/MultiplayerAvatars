using CustomAvatar.Avatar;
using MultiplayerAvatars.Providers.Abstractions;
using MultiplayerAvatars.Providers.Attributes;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerAvatars.Providers
{
    // Checks all avatar providers in order of priority for an avatar, returns the value from the first provider that is not null
    internal class AvatarProviderService : IAvatarProvider
    {
        private readonly IAvatarProvider[] _avatarProviders;

        public AvatarProviderService(
            IAvatarProvider[] avatarProviders)
        {
            Array.Sort(avatarProviders, (a, b) =>
            {
                var aPriority = a.GetType().GetCustomAttribute<AvatarProviderPriorityAttribute>()?.Priority ?? 1000;
                var bPriority = b.GetType().GetCustomAttribute<AvatarProviderPriorityAttribute>()?.Priority ?? 1000;
                return aPriority - bPriority;
            });

            _avatarProviders = avatarProviders;
        }

        public async Task<AvatarPrefab?> GetAvatarByHash(string hash, CancellationToken cancellationToken)
        {
            for (int i = 0; i < _avatarProviders.Length; i++)
            {
                var avatarPrefab = await _avatarProviders[i].GetAvatarByHash(hash, cancellationToken);
                if (avatarPrefab != null)
                    return avatarPrefab;
            }
            return null;
        }
    }
}
