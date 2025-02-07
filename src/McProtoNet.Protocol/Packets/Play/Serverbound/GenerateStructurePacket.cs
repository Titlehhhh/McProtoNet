using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("GenerateStructure", PacketState.Play, PacketDirection.Serverbound)]
    public partial class GenerateStructurePacket : IClientPacket
    {
        public Position Location { get; set; }
        public int Levels { get; set; }
        public bool KeepJigsaws { get; set; }

        [PacketSubInfo(734, 769)]
        public sealed partial class V734_769 : GenerateStructurePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, Levels, KeepJigsaws);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                Position location, int levels, bool keepJigsaws)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteVarInt(levels);
                writer.WriteBoolean(keepJigsaws);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V734_769.IsSupportedVersionStatic(protocolVersion))
                V734_769.SerializeInternal(ref writer, protocolVersion, Location, Levels, KeepJigsaws);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.GenerateStructure), protocolVersion);
        }
    }
}