using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class EditBookPacket : IClientPacket
    {
        public sealed class V393 : EditBookPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, NewBook, Signing);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Slot newBook, bool signing)
            {
                writer.WriteSlot(newBook, protocolVersion);
                writer.WriteBoolean(signing);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion == 393;
            }

            public Slot NewBook { get; set; }
            public bool Signing { get; set; }
        }

        public sealed class V401_755 : EditBookPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, NewBook, Signing, Hand);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Slot newBook, bool signing, int hand)
            {
                writer.WriteSlot(newBook, protocolVersion);
                writer.WriteBoolean(signing);
                writer.WriteVarInt(hand);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 401 and <= 755;
            }

            public Slot NewBook { get; set; }
            public bool Signing { get; set; }
            public int Hand { get; set; }
        }

        public sealed class V756_769 : EditBookPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Hand, Pages, Title);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int hand, string[] pages, string? title)
            {
                writer.WriteVarInt(hand);
                writer.WriteVarInt(pages.Length);
                foreach (var pages_item in pages)
                writer.WriteString(pages_item);
                writer.WriteBoolean(title is not null);
                if (title is not null)
                writer.WriteString((string)title);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 756 and <= 769;
            }

            public int Hand { get; set; }
            public string[] Pages { get; set; }
            public string? Title { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V393.SupportedVersion(protocolVersion) || V401_755.SupportedVersion(protocolVersion) || V756_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393.SupportedVersion(protocolVersion))
                V393.SerializeInternal(ref writer, protocolVersion, default, false);
            else if (V401_755.SupportedVersion(protocolVersion))
                V401_755.SerializeInternal(ref writer, protocolVersion, default, false, 0);
            else if (V756_769.SupportedVersion(protocolVersion))
                V756_769.SerializeInternal(ref writer, protocolVersion, 0, [], null);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.EditBook), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.EditBook;

        public ClientPacket GetPacketId() => PacketId;
    }
}