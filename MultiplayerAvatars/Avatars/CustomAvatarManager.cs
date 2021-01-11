using System;
using Zenject;
using CustomAvatar.Avatar;
using CustomAvatar.Player;
using System.Collections.Generic;
using MultiplayerExtensions.Packets;

namespace MultiplayerAvatars.Avatars
{
    internal class CustomAvatarManager : IInitializable
    {
        private readonly PacketManager _packetManager;
        private readonly FloorController _floorController;
        private readonly PlayerAvatarManager _avatarManager;
        private readonly IMultiplayerSessionManager _sessionManager;
        private readonly IAvatarProvider<LoadedAvatar> _avatarProvider;


        public CustomAvatarData localAvatar = new CustomAvatarData();
        public bool hashesCalculated = false;
        public Action<IConnectedPlayer, CustomAvatarData>? avatarReceived;
        private readonly Dictionary<string, CustomAvatarData> _avatars = new Dictionary<string, CustomAvatarData>();

        internal CustomAvatarManager(PacketManager packetManager, FloorController floorController, PlayerAvatarManager avatarManager, IMultiplayerSessionManager sessionManager, IAvatarProvider<LoadedAvatar> avatarProvider)
        {
            _packetManager = packetManager;
            _avatarManager = avatarManager;
            _sessionManager = sessionManager;
            _avatarProvider = avatarProvider;
            _floorController = floorController;
        }

        public void Initialize()
        {
            Plugin.Log?.Info("Setting up CustomAvatarManager");
            _avatarProvider.hashesCalculated += (x,y) => hashesCalculated = true;
            _avatarManager.avatarChanged += OnAvatarChanged;
            _avatarManager.avatarScaleChanged += SetAvatarScale;
            _floorController.floorPositionChanged += SetAvatarFloorPosition;

            _sessionManager.playerConnectedEvent += OnPlayerConnected;
            _packetManager.RegisterCallback<CustomAvatarPacket>(HandleAvatarPacket);

            OnAvatarChanged(_avatarManager.currentlySpawnedAvatar);
            localAvatar.floor = _floorController.floorPosition;
        }

        private void SetAvatarScale(float scale)
        {
            localAvatar.scale = scale;
        }

        private void SetAvatarFloorPosition(float floor)
        {
            localAvatar.floor = floor;
        }

        public CustomAvatarData? GetAvatarByUserId(string userId)
        {
            if (_avatars.ContainsKey(userId))
                return _avatars[userId];
            return null;
        }

        private void OnAvatarChanged(SpawnedAvatar avatar)
        {
            if (!avatar) return;
            if (!hashesCalculated)
            {
                _avatarProvider.hashesCalculated += (x, y) => OnAvatarChanged(avatar);
                return;
            }

            Plugin.Log?.Warn($"Attempting to hash {avatar.avatar.fullPath}");
            _avatarProvider.HashAvatar(avatar.avatar).ContinueWith(r =>
            {
                localAvatar.hash = r.Result;
                localAvatar.scale = avatar.scale;
            });
        }

        private void OnPlayerConnected(IConnectedPlayer player)
        {
            CustomAvatarPacket localAvatarPacket = localAvatar.GetPacket();
            _packetManager.Send(localAvatarPacket);
        }

        private void HandleAvatarPacket(CustomAvatarPacket packet, IConnectedPlayer player)
        {
            Plugin.Log?.Info($"Received 'CustomAvatarPacket' from '{player.userId}' with '{packet.hash}'");
            _avatars[player.userId] = new CustomAvatarData(packet);
            avatarReceived?.Invoke(player, _avatars[player.userId]);
        }
    }
}
