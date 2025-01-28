using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SpawnPosition", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SpawnPositionPacket : IServerPacket
    {
        public Position Location { get; set; }

        [PacketSubInfo(340, 754)]
        public sealed partial class V340_754 : SpawnPositionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
            }
        }

        [PacketSubInfo(755, 769)]
        public sealed partial class V755_769 : SpawnPositionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                Angle = reader.ReadFloat();
            }

            public float Angle { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}