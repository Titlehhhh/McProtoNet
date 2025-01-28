using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("UpdateCommandBlockMinecart", PacketState.Play, PacketDirection.Serverbound)]
    public partial class UpdateCommandBlockMinecartPacket : IClientPacket
    {
        public int EntityId { get; set; }
        public string Command { get; set; }
        public bool TrackOutput { get; set; }

        [PacketSubInfo(393, 769)]
        public sealed partial class V393_769 : UpdateCommandBlockMinecartPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, EntityId, Command, TrackOutput);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int entityId, string command, bool trackOutput)
            {
                writer.WriteVarInt(entityId);
                writer.WriteString(command);
                writer.WriteBoolean(trackOutput);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393_769.IsSupportedVersionStatic(protocolVersion))
                V393_769.SerializeInternal(ref writer, protocolVersion, EntityId, Command, TrackOutput);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.UpdateCommandBlockMinecart), protocolVersion);
        }
    }
}