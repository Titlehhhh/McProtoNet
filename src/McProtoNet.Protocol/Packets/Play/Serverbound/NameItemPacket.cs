using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("NameItem", PacketState.Play, PacketDirection.Serverbound)]
    public partial class NameItemPacket : IClientPacket
    {
        public string Name { get; set; }

        [PacketSubInfo(393, 769)]
        public sealed partial class V393_769 : NameItemPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Name);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, string name)
            {
                writer.WriteString(name);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393_769.IsSupportedVersionStatic(protocolVersion))
                V393_769.SerializeInternal(ref writer, protocolVersion, Name);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.NameItem), protocolVersion);
        }
    }
}