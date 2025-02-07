using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("TabComplete", PacketState.Play, PacketDirection.Serverbound)]
    public partial class TabCompletePacket : IClientPacket
    {
        public string Text { get; set; }

        [PacketSubInfo(340, 340)]
        public sealed partial class V340 : TabCompletePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Text, AssumeCommand, LookedAtBlock);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                string text, bool assumeCommand, Position? lookedAtBlock)
            {
                writer.WriteString(text);
                writer.WriteBoolean(assumeCommand);
                writer.WriteBoolean(lookedAtBlock is not null);
                if (lookedAtBlock is not null)
                    writer.WritePosition((Position)lookedAtBlock, protocolVersion);
            }

            public bool AssumeCommand { get; set; }
            public Position? LookedAtBlock { get; set; }
        }

        [PacketSubInfo(351, 769)]
        public sealed partial class V351_769 : TabCompletePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, TransactionId, Text);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                int transactionId, string text)
            {
                writer.WriteVarInt(transactionId);
                writer.WriteString(text);
            }

            public int TransactionId { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340.IsSupportedVersionStatic(protocolVersion))
                V340.SerializeInternal(ref writer, protocolVersion, Text, false, null);
            else if (V351_769.IsSupportedVersionStatic(protocolVersion))
                V351_769.SerializeInternal(ref writer, protocolVersion, 0, Text);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.TabComplete), protocolVersion);
        }
    }
}