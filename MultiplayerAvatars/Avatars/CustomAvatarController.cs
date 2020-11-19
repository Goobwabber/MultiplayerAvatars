using CustomAvatar.Avatar;
using System.Linq;
using System.Threading;
using UnityEngine;
using Zenject;

namespace MultiplayerAvatars.Avatars
{
    internal class CustomAvatarController : MonoBehaviour
    {
        private AvatarSpawner _avatarSpawner;
        protected IConnectedPlayer _connectedPlayer;
        private CustomAvatarManager _customAvatarManager;
        private IAvatarProvider<LoadedAvatar> _avatarProvider;

        private CustomAvatarData avatarData;
        private LoadedAvatar? loadedAvatar;
        private SpawnedAvatar? spawnedAvatar;
        private AvatarPoseController poseController;

        [Inject]
        public void Construct(AvatarSpawner avatarSpawner, IAvatarProvider<LoadedAvatar> avatarProvider, [InjectOptional] IConnectedPlayer connectedPlayer, CustomAvatarManager customAvatarManager)
        {
            _avatarSpawner = avatarSpawner;
            _avatarProvider = avatarProvider;
            _connectedPlayer = connectedPlayer;
            _customAvatarManager = customAvatarManager;
        }

        public virtual void Start()
        {
            _customAvatarManager.avatarReceived += OnAvatarReceived;

            TryGetPoseController();
        }

        public virtual void Update()
        {
            if (poseController == null)
            {
                TryGetPoseController();
            }
        }

        public void TryGetPoseController()
        {
            var poseControllers = gameObject.GetComponentsInChildren<AvatarPoseController>();
            if(poseControllers.Length != 0)
            {
                poseController = poseControllers.First();
                CustomAvatarData avatar = _customAvatarManager.GetAvatarByUserId(_connectedPlayer.userId);
                if (avatar != null)
                    OnAvatarReceived(_connectedPlayer, avatar);
            }
        }

        private void OnAvatarReceived(IConnectedPlayer player, CustomAvatarData avatar)
        {
            if (player.userId != _connectedPlayer.userId)
                return;

            if (avatar == null)
                return;

            if (avatar.hash == new CustomAvatarData().hash)
                return;

            avatarData = avatar;
            _avatarProvider.FetchAvatarByHash(avatar.hash, CancellationToken.None).ContinueWith(a =>
            {
                if (!a.IsFaulted && a.Result is LoadedAvatar)
                {
                    HMMainThreadDispatcher.instance.Enqueue(() =>
                    {
                        CreateAvatar(a.Result);
                    });
                }
            });
        }

        private void CreateAvatar(LoadedAvatar avatar)
        {
            loadedAvatar = avatar;
            if (spawnedAvatar != null)
                Destroy(spawnedAvatar);

            spawnedAvatar = _avatarSpawner.SpawnAvatar(avatar, new MultiplayerAvatarInput(poseController, transform.name != "MultiplayerLobbyAvatar(Clone)"), poseController.transform);
            spawnedAvatar.SetLocomotionEnabled(true);
            spawnedAvatar.scale = avatarData.scale;
        }
    }
}
