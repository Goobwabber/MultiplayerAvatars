using CustomAvatar.Avatar;
using CustomAvatar.Player;
using MultiplayerAvatars.Providers;
using MultiplayerCore.Networking;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zenject;

namespace MultiplayerAvatars.Networking
{
    internal class CustomAvatarManager : IInitializable, IDisposable
    {
        public Action<IConnectedPlayer, CustomAvatarPacket>? avatarReceived;

        private CustomAvatarPacket _localPlayerAvatar = new();
        private Dictionary<string, CustomAvatarPacket> _connectedPlayerAvatars = new();

        private readonly MpPacketSerializer _packetSerializer;
        private readonly PlayerAvatarManager _playerAvatarManager;
        private readonly CustomAvatarsProvider _customAvatarsProvider;
        private readonly IMultiplayerSessionManager _sessionManager;
        private readonly SiraLog _logger;

        public CustomAvatarManager(
            MpPacketSerializer packetSerializer,
            PlayerAvatarManager playerAvatarManager,
            CustomAvatarsProvider customAvatarsProvider,
            IMultiplayerSessionManager sessionManager,
            SiraLog logger)
        {
            _packetSerializer = packetSerializer;
            _playerAvatarManager = playerAvatarManager;
            _customAvatarsProvider = customAvatarsProvider;
            _sessionManager = sessionManager;
            _logger = logger;
        }

        public void Initialize()
        {
            _playerAvatarManager.avatarChanged += HandleAvatarChanged;
            _playerAvatarManager.avatarScaleChanged += HandleAvatarScaleChanged;
            _sessionManager.playerConnectedEvent += HandlePlayerConnected;
            _sessionManager.playerDisconnectedEvent += HandlePlayerDisconnected;

            Task.Run(() => UpdateAvatarHash(_playerAvatarManager.currentlySpawnedAvatar.prefab.fullPath));
            _localPlayerAvatar.Scale = _playerAvatarManager.currentlySpawnedAvatar?.scale ?? 1f;

            _packetSerializer.RegisterCallback<CustomAvatarPacket>(HandleCustomAvatarPacket);
        }

        public void Dispose()
        {
            _playerAvatarManager.avatarChanged -= HandleAvatarChanged;
            _playerAvatarManager.avatarScaleChanged -= HandleAvatarScaleChanged;
            _sessionManager.playerConnectedEvent -= HandlePlayerConnected;
            _sessionManager.playerDisconnectedEvent -= HandlePlayerDisconnected;

            _packetSerializer.UnregisterCallback<CustomAvatarPacket>();
        }

        public CustomAvatarPacket GetPlayerAvatarPacket(string userId)
        {
            if (!_connectedPlayerAvatars.ContainsKey(userId))
                return new();
            return _connectedPlayerAvatars[userId];
        }

        private void HandleAvatarChanged(SpawnedAvatar avatar)
        {
            if (avatar == null)
            {
                _localPlayerAvatar = new();
                _sessionManager.Send(_localPlayerAvatar);
                return;
            }

            Task.Run(() => UpdateAvatarHash(avatar.prefab.fullPath));
        }

        private void HandleAvatarScaleChanged(float scale)
        {
            _localPlayerAvatar.Scale = scale;
            _sessionManager.Send(_localPlayerAvatar);
        }

        private void HandleCustomAvatarPacket(CustomAvatarPacket packet, IConnectedPlayer player)
        {
            _logger.Debug($"Received 'CustomAvatarPacket' from '{player.userId}' with '{packet.Hash}'");
            _connectedPlayerAvatars[player.userId] = packet;
            avatarReceived?.Invoke(player, packet);
        }

        private void HandlePlayerConnected(IConnectedPlayer player)
        {
            _sessionManager.Send(_localPlayerAvatar);
        }

        private void HandlePlayerDisconnected(IConnectedPlayer player)
        {
            _connectedPlayerAvatars.Remove(player.userId);
        }

        private async Task UpdateAvatarHash(string path)
        {
            var hash = await _customAvatarsProvider.GetAvatarHash(path);
            if (hash == null)
            {
                _logger.Warn($"Local player switched to an avatar that was not hashed: {path}");
                return;
            }

            _localPlayerAvatar.Hash = hash;
            _sessionManager.Send(_localPlayerAvatar);
        }
    }
}
