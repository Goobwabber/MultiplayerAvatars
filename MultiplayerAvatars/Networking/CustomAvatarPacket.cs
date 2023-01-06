using LiteNetLib.Utils;
using MultiplayerCore.Networking.Abstractions;

namespace MultiplayerAvatars.Networking
{
    internal class CustomAvatarPacket : MpPacket
    {
        public string Hash { get; set; } = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
        public float Scale { get; set; } = 1f;

        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(Hash);
            writer.Put(Scale);
        }

        public override void Deserialize(NetDataReader reader)
        {
            Hash = reader.GetString();
            Scale = reader.GetFloat();
        }
    }
}
