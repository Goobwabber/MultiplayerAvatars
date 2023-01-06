using CustomAvatar.Avatar;
using CustomAvatar.Player;
using MultiplayerAvatars.Providers.Abstractions;
using MultiplayerAvatars.Providers.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerAvatars.Providers
{
    // Avatar provider for local avatars
    [AvatarProviderPriority(100)]
    internal class CustomAvatarsProvider : IAvatarProvider
    {
        public string CustomAvatarsPath = PlayerAvatarManager.kCustomAvatarsPath;

        private Task _updateAvatarHashes => Task.Run(UpdateAvatarHashes);
        private Dictionary<string, string> _cachedAvatarPaths = new();
        private Dictionary<string, string> _cachedAvatarHashes = new();

        private readonly AvatarLoader _avatarLoader;

        public CustomAvatarsProvider(
            AvatarLoader avatarLoader)
        {
            _avatarLoader = avatarLoader;
        }

        public async Task<AvatarPrefab?> GetAvatarByHash(string hash, CancellationToken cancellationToken)
        {
            await _updateAvatarHashes.ConfigureAwait(false);
            if (!_cachedAvatarPaths.TryGetValue(hash, out string path))
                return null;
            return await _avatarLoader.LoadFromFileAsync(path, null, cancellationToken);
        }

        public string? GetCachedAvatarHash(string path)
        {
            if (!_cachedAvatarHashes.ContainsKey(path))
                return null;
            return _cachedAvatarHashes[path];
        }

        public void AddAvatar(string path, string hash)
        {
            if (_cachedAvatarPaths.ContainsKey(hash))
                return;
            _cachedAvatarPaths[hash] = path;
            _cachedAvatarHashes[path] = hash;
        }

        public void AddAvatar(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                string hash = BitConverter.ToString(MD5.Create().ComputeHash(fs)).Replace("-", "");
                if (_cachedAvatarPaths.ContainsKey(hash))
                    return;
                _cachedAvatarPaths[hash] = path;
                _cachedAvatarHashes[path] = hash;
            }
        }

        public void UpdateAvatarHashes()
        {
            var avatarFiles = Directory.GetFiles(CustomAvatarsPath, "*.avatar");
            foreach(string avatarFile in avatarFiles)
            {
                using (var fs = File.OpenRead(avatarFile))
                {
                    string hash = BitConverter.ToString(MD5.Create().ComputeHash(fs)).Replace("-", "");
                    if (_cachedAvatarPaths.ContainsKey(hash))
                        return;
                    _cachedAvatarPaths[hash] = avatarFile;
                    _cachedAvatarHashes[avatarFile] = hash;
                }
            }
        }
    }
}
