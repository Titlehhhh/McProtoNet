using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("ChatSuggestions", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ChatSuggestionsPacket : IServerPacket
    {
        public int Action { get; set; }
        public string[] Entries { get; set; }

        [PacketSubInfo(760, 769)]
        public sealed partial class V760_769 : ChatSuggestionsPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Action = reader.ReadVarInt();
                Entries = reader.ReadArray<string, StringArrayReader>(LengthFormat.VarInt);
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}