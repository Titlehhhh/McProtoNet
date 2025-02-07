using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("DamageEvent", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class DamageEventPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public int SourceTypeId { get; set; }
        public int SourceCauseId { get; set; }
        public int SourceDirectId { get; set; }
        public Vector3F64? SourcePosition { get; set; }

        [PacketSubInfo(762, 769)]
        public sealed partial class V762_769 : DamageEventPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                SourceTypeId = reader.ReadVarInt();
                SourceCauseId = reader.ReadVarInt();
                SourceDirectId = reader.ReadVarInt();
                SourcePosition = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) =>
                    r_0.ReadVector3F64(protocolVersion));
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}