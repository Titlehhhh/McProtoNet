using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("LockDifficulty", PacketState.Play, PacketDirection.Serverbound)]
    public partial class LockDifficultyPacket : IClientPacket
    {
        public bool Locked { get; set; }

        [PacketSubInfo(477, 769)]
        public sealed partial class V477_769 : LockDifficultyPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Locked);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, bool locked)
            {
                writer.WriteBoolean(locked);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V477_769.IsSupportedVersionStatic(protocolVersion))
                V477_769.SerializeInternal(ref writer, protocolVersion, Locked);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.LockDifficulty), protocolVersion);
        }
    }
}