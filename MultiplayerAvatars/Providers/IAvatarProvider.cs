﻿using MultiplayerAvatars.Avatars;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerAvatars.Providers
{
    public interface IAvatarProvider
    {
        event EventHandler<AvatarDownloadedEventArgs>? avatarDownloaded;
        event EventHandler? hashesCalculated;
        Type AvatarType { get; }
        bool isCalculatingHashes { get; }
        int cachedAvatarsCount { get; }
        string AvatarDirectory { get; }
        Task HashAllAvatars(string directory);
        bool CacheAvatar(string avatarPath);
        Task<AvatarInfo?> FetchAvatarInfoByHash(string hash, CancellationToken cancellationToken);
    }

    public interface IAvatarProvider<T> : IAvatarProvider where T : class
    {
        bool TryGetCachedAvatar(string hash, out T? avatar);
        Task<T?> LoadAvatar(string avatarPath);
        Task<string> HashAvatar(T avatar);
        Task<T?> FetchAvatarByHash(string hash, CancellationToken cancellationToken);
    }

    public class AvatarDownloadedEventArgs : EventArgs
    {
        public readonly string Hash;

        public AvatarDownloadedEventArgs(string hash)
        {
            Hash = hash;
        }
    }
}
