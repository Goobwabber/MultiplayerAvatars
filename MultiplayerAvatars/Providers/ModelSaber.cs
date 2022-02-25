﻿using CustomAvatar.Avatar;
using CustomAvatar.Player;
using MultiplayerAvatars.Avatars;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Zenject;

namespace MultiplayerAvatars.Providers
{
    public class ModelSaber : IAvatarProvider<AvatarPrefab>, IInitializable
    {
        [Inject]
        private AvatarLoader? _avatarLoader;

        public event EventHandler<AvatarDownloadedEventArgs>? avatarDownloaded;
        public event EventHandler? hashesCalculated;
        public Type AvatarType => typeof(AvatarPrefab);
        public bool isCalculatingHashes { get; protected set; }
        public int cachedAvatarsCount => cachedAvatars.Count;
        public string AvatarDirectory => PlayerAvatarManager.kCustomAvatarsPath;

        private Dictionary<string, AvatarPrefab> cachedAvatars = new Dictionary<string, AvatarPrefab>();

        public bool CacheAvatar(string avatarPath)
        {
            return false;
        }

        public bool TryGetCachedAvatar(string hash, out AvatarPrefab? avatar)
        {
            return cachedAvatars.TryGetValue(hash, out avatar);
        }

        public async Task<AvatarPrefab?> FetchAvatarByHash(string hash, CancellationToken cancellationToken)
        {
            if (cachedAvatars.TryGetValue(hash, out AvatarPrefab cachedAvatar))
                return cachedAvatar;
            var avatarInfo = await FetchAvatarInfoByHash(hash, cancellationToken);
            if (avatarInfo == null)
            {
                Plugin.Log?.Info($"Couldn't find avatar with hash '{hash}'");
                return null;
            }
            var path = await avatarInfo.DownloadAvatar(cancellationToken);
            if (path != null)
                return await LoadAvatar(path);
            else
                return null;
        }

        public async Task<AvatarInfo?> FetchAvatarInfoByHash(string hash, CancellationToken cancellationToken)
        {
            AvatarInfo? avatarInfo = null;
            try
            {
                Uri uri = new Uri($"https://modelsaber.com/api/v1/avatar/get.php?filter=hash:{hash}");
                var response = await Plugin.HttpClient.GetAsync(uri, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    Plugin.Log.Debug("Received response from ModelSaber...");
                    string content = await response.Content.ReadAsStringAsync();
                    Dictionary<string, AvatarInfo> avatars = JsonConvert.DeserializeObject<Dictionary<string, AvatarInfo>>(content);
                    if (avatars.Count() == 0)
                    {
                        return null;
                    }
                    avatarInfo = avatars.First().Value;
                }
                else
                {
                    Plugin.Log?.Warn($"Unable to retrieve avatar info from ModelSaber: {response.StatusCode}|{response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error retrieving avatar info for '{hash}': {ex.Message}");
                Plugin.Log?.Debug(ex);
            }
            return avatarInfo;
        }

        public Task<string> HashAvatar(AvatarPrefab avatar)
        {
            var path = avatar?.fullPath ?? throw new ArgumentNullException(nameof(avatar));
            string fullPath = Path.Combine(Path.GetFullPath("CustomAvatars"), path);
            if (!File.Exists(fullPath))
                throw new ArgumentException($"File at {fullPath} does not exist.");
            return Task.Run(() =>
            {
                string hash = null!;
                Plugin.Log?.Debug($"Hashing avatar path {path}");
                using (var fs = File.OpenRead(fullPath))
                {
                    hash = BitConverter.ToString(MD5.Create().ComputeHash(fs)).Replace("-", "");
                    if (!cachedAvatars.ContainsKey(hash))
                        cachedAvatars.Add(hash, avatar);
                    return hash;
                }
            });
        }

        public async Task HashAllAvatars(string directory)
        {
            //var avatarFiles = Directory.GetFiles(PlayerAvatarManager.kCustomAvatarsPath, "*.avatar");
            var avatarFiles = Directory.GetFiles(AvatarDirectory, "*.avatar");
            Plugin.Log?.Debug($"Hashing avatars... {avatarFiles.Length} possible avatars found");
            cachedAvatars.Clear();
            foreach (string avatarFile in avatarFiles)
            {
                await LoadAvatar(avatarFile);
            }
            isCalculatingHashes = false;
            Plugin.Log?.Debug($"{cachedAvatarsCount} avatars hashed and loaded!");
            HMMainThreadDispatcher.instance.Enqueue(() =>
            {
                hashesCalculated?.Invoke(this, EventArgs.Empty);
            });
        }

        public async Task<AvatarPrefab?> LoadAvatar(string avatarFile)
        {
            TaskCompletionSource<AvatarPrefab?> tcs = new TaskCompletionSource<AvatarPrefab?>();
            try
            {
                _ = _avatarLoader ?? throw new InvalidOperationException("Avatar loader not set.");

                var coroutine = _avatarLoader.LoadFromFileAsync(avatarFile, (AvatarPrefab avatar) => tcs.TrySetResult(avatar), e => tcs.TrySetException(e));
                await IPA.Utilities.Async.Coroutines.AsTask(coroutine);
                if (!tcs.Task.IsCompleted)
                {
                    var timeout = Task.Delay(10000);
                    var task = await Task.WhenAny(tcs.Task, timeout);
                    if (task == timeout)
                    {
                        Plugin.Log?.Warn($"Timeout exceeded trying to load avatar '{avatarFile}'");
                        tcs.TrySetCanceled();
                        return null;
                    }
                }
                AvatarPrefab? avatar = await tcs.Task;
                if (avatar == null)
                {
                    Plugin.Log?.Warn($"Couldn't load avatar at '{avatarFile}'");
                    return null;
                }
                try
                {
                    string calculatedHash = await HashAvatar(avatar);
                    Plugin.Log?.Debug($"Hashed avatar \"{avatar.descriptor.name}\"! Hash: {calculatedHash}");
                }
                catch (Exception ex)
                {
                    Plugin.Log?.Error($"Unable to hash avatar \"{avatar.descriptor.name}\"! Exception: {ex}");
                    Plugin.Log?.Debug(ex);
                }
                return avatar;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Unable to load avatar!");
                Plugin.Log?.Debug(ex);
            }
            return null;
        }

        public void Initialize()
        {
            _ = InitializeTask();
        }

        private async Task InitializeTask()
        {
            try
            {
                await HashAllAvatars(AvatarDirectory);
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex);
            }
        }
    }
}
