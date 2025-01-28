using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("EditBook", PacketState.Play, PacketDirection.Serverbound)]
    public partial class EditBookPacket : IClientPacket
    {
        [PacketSubInfo(393, 393)]
        public sealed partial class V393 : EditBookPacket
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

            public Slot NewBook { get; set; }
            public bool Signing { get; set; }
        }

        [PacketSubInfo(401, 755)]
        public sealed partial class V401_755 : EditBookPacket
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

            public Slot NewBook { get; set; }
            public bool Signing { get; set; }
            public int Hand { get; set; }
        }

        [PacketSubInfo(756, 769)]
        public sealed partial class V756_769 : EditBookPacket
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

            public int Hand { get; set; }
            public string[] Pages { get; set; }
            public string? Title { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393.IsSupportedVersionStatic(protocolVersion))
                V393.SerializeInternal(ref writer, protocolVersion, default, false);
            else if (V401_755.IsSupportedVersionStatic(protocolVersion))
                V401_755.SerializeInternal(ref writer, protocolVersion, default, false, 0);
            else if (V756_769.IsSupportedVersionStatic(protocolVersion))
                V756_769.SerializeInternal(ref writer, protocolVersion, 0, [], null);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.EditBook), protocolVersion);
        }
    }
}