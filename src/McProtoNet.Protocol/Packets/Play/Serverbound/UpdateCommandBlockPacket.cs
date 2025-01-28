using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("UpdateCommandBlock", PacketState.Play, PacketDirection.Serverbound)]
    public partial class UpdateCommandBlockPacket : IClientPacket
    {
        public Position Location { get; set; }
        public string Command { get; set; }
        public int Mode { get; set; }
        public byte Flags { get; set; }

        [PacketSubInfo(393, 769)]
        public sealed partial class V393_769 : UpdateCommandBlockPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, Command, Mode, Flags);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Position location, string command, int mode, byte flags)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteString(command);
                writer.WriteVarInt(mode);
                writer.WriteUnsignedByte(flags);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393_769.IsSupportedVersionStatic(protocolVersion))
                V393_769.SerializeInternal(ref writer, protocolVersion, Location, Command, Mode, Flags);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.UpdateCommandBlock), protocolVersion);
        }
    }
}