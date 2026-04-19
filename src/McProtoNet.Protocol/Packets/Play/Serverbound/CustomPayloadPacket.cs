using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using System;
using McProtoNet.Minecraft;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("CustomPayload", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x0B)]
[PacketId(751, 754, 0x0B)]
[PacketId(755, 758, 0x0A)]
[PacketId(759, 759, 0x0C)]
[PacketId(760, 760, 0x0D)]
[PacketId(761, 761, 0x0C)]
[PacketId(762, 763, 0x0D)]
[PacketId(764, 764, 0x0F)]
[PacketId(765, 765, 0x10)]
[PacketId(766, 767, 0x12)]
[PacketId(768, 770, 0x14)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x15)]
public sealed partial class CustomPayloadPacket : IPacket
{
    public string Channel { get; set; } = string.Empty;
    public byte[]? Data { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Channel);
        if (Data != null)
        {
            writer.WriteBuffer(Data);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Channel = reader.ReadString();
        Data = reader.ReadRestBuffer();
    }
}