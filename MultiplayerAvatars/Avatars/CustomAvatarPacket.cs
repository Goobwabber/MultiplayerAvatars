﻿using LiteNetLib.Utils;

namespace MultiplayerAvatars.Avatars
{
    public class CustomAvatarPacket : INetSerializable, IPoolablePacket
    {
        public void Release() => ThreadStaticPacketPool<CustomAvatarPacket>.pool.Release(this);

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(hash);
            writer.Put(scale);
            writer.Put(floor);
        }

        public void Deserialize(NetDataReader reader)
        {
            hash = reader.GetString();
            scale = reader.GetFloat();
            floor = reader.GetFloat();
        }

        public CustomAvatarPacket Init(string avatarHash, float avatarScale, float avatarFloor)
        {
            this.hash = avatarHash;
            this.scale = avatarScale;
            this.floor = avatarFloor;

            return this;
        }

        public string hash = null!;
        public float scale;
        public float floor;
    }
}
