namespace MultiplayerAvatars.Avatars
{
    class CustomAvatarData
    {
        public string hash = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
        public float scale = 1f;
        public float floor = 0f;

        public CustomAvatarData() { }
        public CustomAvatarData(CustomAvatarPacket packet)
        {
            hash = packet.hash;
            scale = packet.scale;
            floor = packet.floor;
        }

        public CustomAvatarPacket GetPacket()
        {
            return new CustomAvatarPacket().Init(hash, scale, floor);
        }
    }
}
