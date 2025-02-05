using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("DebugSample", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class DebugSamplePacket : IServerPacket
    {
        public long[] Sample { get; set; }
        public int Type { get; set; }

        [PacketSubInfo(766, 769)]
        public sealed partial class V766_769 : DebugSamplePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Sample = reader.ReadArray<long, LongArrayReader>(LengthFormat.VarInt);
                Type = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}