using CustomAvatar.Avatar;
using CustomAvatar.Player;
using MultiplayerAvatars.Providers.Abstractions;
using MultiplayerAvatars.Providers.Attributes;
using MultiplayerAvatars.Providers.ModelSaber;
using Newtonsoft.Json;
using SiraUtil.Logging;
using SiraUtil.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerAvatars.Providers
{
    // Avatar provider for ModelSaber avatars
    [AvatarProviderPriority(500)]
    internal class ModelSaberProvider : IAvatarProvider
    {
        public string CustomAvatarsPath = PlayerAvatarManager.kCustomAvatarsPath;

        private readonly IHttpService _httpService;
        private readonly AvatarLoader _avatarLoader;
        private readonly CustomAvatarsProvider _customAvatarsProvider;
        private readonly SiraLog _logger;
        private static readonly JsonSerializer _jsonSerializer = new();

        public ModelSaberProvider(
            IHttpService httpService,
            AvatarLoader avatarLoader,
            CustomAvatarsProvider customAvatarsProvider,
            SiraLog logger)
        {
            _httpService = httpService;
            _avatarLoader = avatarLoader;
            _customAvatarsProvider = customAvatarsProvider;
            _logger = logger;
        }

        public async Task<AvatarPrefab?> GetAvatarByHash(string hash, CancellationToken cancellationToken)
        {
            ModelInfo? modelInfo = await FetchModelInfoByHash(hash, cancellationToken);
            if (modelInfo == null)
                return null;

            string? avatarPath = await DownloadAvatar(modelInfo, cancellationToken);
            if (avatarPath == null)
                return null;

            _customAvatarsProvider.AddAvatar(avatarPath, hash);

            return await _avatarLoader.LoadFromFileAsync(avatarPath, null, cancellationToken);
        }

        public async Task<ModelInfo?> FetchModelInfoByHash(string hash, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpService.GetAsync($"https://modelsaber.com/api/v2/get.php?filter=hash:{hash}", cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!response.Successful)
                {
                    _logger.Warn($"Unable to retrieve model info from ModelSaber: {response.Code}|{await response.Error()}");
                    return null;
                }

                _logger.Debug($"Received response from ModelSaber...");
                using (StringReader reader = new StringReader(await response.ReadAsStringAsync().ConfigureAwait(false)))
                {
                    using (JsonTextReader jsonTextReader = new JsonTextReader(reader))
                    {
                        return _jsonSerializer.Deserialize<Dictionary<string, ModelInfo>>(jsonTextReader).FirstOrDefault().Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error retrieving model info for '{hash}': {ex.Message}");
                _logger.Debug(ex);
            }
            return null;
        }

        public async Task<string?> DownloadAvatar(ModelInfo modelInfo, CancellationToken cancellationToken)
        {
            string customAvatarPath = null!;
            try
            {
                if (string.IsNullOrWhiteSpace(modelInfo.Name))
                {
                    _logger.Warn($"Tried to download model with no name: {modelInfo.Hash}");
                    return null;
                }

                var response = await _httpService.GetAsync(modelInfo.DownloadUrl, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (!response.Successful)
                {
                    _logger.Warn($"Unable to download model from ModelSaber: {response.Code}|{await response.Error()}");
                    return null;
                }

                _logger.Debug($"Received response from ModelSaber...");
                Directory.CreateDirectory(CustomAvatarsPath);

                customAvatarPath = Path.Combine(CustomAvatarsPath, $"{modelInfo.Name}.avatar");
                for (int i = 2; File.Exists(customAvatarPath); i++)
                    customAvatarPath = Path.Combine(CustomAvatarsPath, $"{modelInfo.Name}_{i}.avatar");

                using (var fs = File.Create(customAvatarPath))
                {
                    var ds = await response.ReadAsStreamAsync();
                    await ds.CopyToAsync(fs).ConfigureAwait(false);
                }

                return customAvatarPath;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error downloading avatar from '{modelInfo.DownloadUrl}': {ex.Message}");
                _logger.Debug(ex);
                if (customAvatarPath != null && File.Exists(customAvatarPath))
                {
                    try
                    {
                        File.Delete(customAvatarPath);
                    }
                    catch (Exception e)
                    {
                        _logger.Error($"Error trying to delete incomplete download at '{customAvatarPath}': {e.Message}");
                    }
                }
            }

            return null;
        }
    }
}
