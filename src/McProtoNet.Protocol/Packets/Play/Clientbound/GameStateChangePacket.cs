using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("GameStateChange", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class GameStateChangePacket : IServerPacket
    {
        public byte Reason { get; set; }
        public float GameMode { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : GameStateChangePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Reason = reader.ReadUnsignedByte();
                GameMode = reader.ReadFloat();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}