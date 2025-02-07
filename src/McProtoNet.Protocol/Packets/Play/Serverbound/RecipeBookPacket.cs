using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("RecipeBook", PacketState.Play, PacketDirection.Serverbound)]
    public partial class RecipeBookPacket : IClientPacket
    {
        public int BookId { get; set; }
        public bool BookOpen { get; set; }
        public bool FilterActive { get; set; }

        [PacketSubInfo(751, 769)]
        public sealed partial class V751_769 : RecipeBookPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, BookId, BookOpen, FilterActive);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int bookId,
                bool bookOpen, bool filterActive)
            {
                writer.WriteVarInt(bookId);
                writer.WriteBoolean(bookOpen);
                writer.WriteBoolean(filterActive);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V751_769.IsSupportedVersionStatic(protocolVersion))
                V751_769.SerializeInternal(ref writer, protocolVersion, BookId, BookOpen, FilterActive);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.RecipeBook), protocolVersion);
        }
    }
}