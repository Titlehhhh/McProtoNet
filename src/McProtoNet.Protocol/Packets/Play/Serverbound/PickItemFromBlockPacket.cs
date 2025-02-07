using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("PickItemFromBlock", PacketState.Play, PacketDirection.Serverbound)]
    public partial class PickItemFromBlockPacket : IClientPacket
    {
        public Position Position { get; set; }
        public bool IncludeData { get; set; }

        [PacketSubInfo(769, 769)]
        public sealed partial class V769 : PickItemFromBlockPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Position, IncludeData);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                Position position, bool includeData)
            {
                writer.WritePosition(position, protocolVersion);
                writer.WriteBoolean(includeData);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V769.IsSupportedVersionStatic(protocolVersion))
                V769.SerializeInternal(ref writer, protocolVersion, Position, IncludeData);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.PickItemFromBlock), protocolVersion);
        }
    }
}