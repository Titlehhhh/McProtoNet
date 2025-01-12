using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class ChatSuggestionsPacket : IServerPacket
    {
        public int Action { get; set; }
        public string[] Entries { get; set; }

        public sealed class V760_769 : ChatSuggestionsPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Action = reader.ReadVarInt();
                Entries = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.String);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 760 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V760_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.ChatSuggestions;

        public ServerPacket GetPacketId() => PacketId;
    }
}