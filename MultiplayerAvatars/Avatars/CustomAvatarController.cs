using CustomAvatar.Avatar;
using MultiplayerAvatars.Networking;
using MultiplayerAvatars.Providers;
using SiraUtil.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace MultiplayerAvatars.Avatars
{
    internal class CustomAvatarController : MonoBehaviour, IInitializable, IDisposable
    {

        private CustomAvatarPacket _avatarPacket = new();
        private AvatarPrefab? _loadedAvatar;
        private SpawnedAvatar? _spawnedAvatar;

        private AvatarSpawner _avatarSpawner = null!;
        private IConnectedPlayer _connectedPlayer = null!;
        private CustomAvatarManager _customAvatarManager = null!;
        private AvatarProviderService _avatarProvider = null!;
        private AvatarPoseController _poseController = null!;
        private MultiplayerAvatarInput _avatarInput = null!;
        private SiraLog _logger = null!;

        [Inject]
        public void Construct(
            AvatarSpawner avatarSpawner,
            IConnectedPlayer connectedPlayer,
            CustomAvatarManager customAvatarManager,
            AvatarProviderService avatarProvider,
            AvatarPoseController poseController,
            SiraLog logger)
        {
            _avatarSpawner = avatarSpawner;
            _avatarProvider = avatarProvider;
            _connectedPlayer = connectedPlayer;
            _customAvatarManager = customAvatarManager;
            _poseController = poseController;
            _logger = logger;

            _avatarInput = new MultiplayerAvatarInput(poseController, !transform.name.Contains("MultiplayerLobbyAvatar"));
        }

        public void Initialize()
        {
            _customAvatarManager.avatarReceived += HandleAvatarReceived;
            _avatarPacket = _customAvatarManager.GetPlayerAvatarPacket(_connectedPlayer.userId);
        }

        public void Dispose()
        {
            _customAvatarManager.avatarReceived -= HandleAvatarReceived;
        }

        private void HandleAvatarReceived(IConnectedPlayer player, CustomAvatarPacket packet)
        {
            if (player.userId != _connectedPlayer.userId)
                return;
            if (packet.Hash == "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")
                return;

            _avatarPacket = packet;
            Task.Run(LoadAvatar);
        }

        private async Task LoadAvatar()
        {
            var avatarPrefab = await _avatarProvider.GetAvatarByHash(_avatarPacket.Hash, CancellationToken.None);
            if (avatarPrefab == null)
            {
                _logger.Warn($"Tried to load avatar and failed: {_avatarPacket.Hash}");
                return;
            }

            HMMainThreadDispatcher.instance.Enqueue(() => CreateAvatar(avatarPrefab));
        }

        private void CreateAvatar(AvatarPrefab avatar)
        {
            _loadedAvatar = avatar;
            if (_spawnedAvatar != null)
                Destroy(_spawnedAvatar);

            _spawnedAvatar = _avatarSpawner.SpawnAvatar(avatar, _avatarInput, _poseController.transform);
            _spawnedAvatar.GetComponent<AvatarIK>().isLocomotionEnabled = true;
            _spawnedAvatar.scale = _avatarPacket.Scale;
        }
    }
}
