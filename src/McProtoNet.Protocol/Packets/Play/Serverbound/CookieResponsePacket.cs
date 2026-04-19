using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[PacketInfo("CookieResponse", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(766, 767, 0x11)]
[PacketId(768, 770, 0x13)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x14)]
public sealed partial class CookieResponsePacket : IPacket
{
    public string Key { get; set; } = "";
    public byte[]? Value { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteString(Key);
                if (Value is null)
                {
                    writer.WriteVarInt(-1);
                }
                else
                {
                    writer.WriteVarInt(Value.Length);
                    writer.WriteBuffer(Value.AsSpan());
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CookieResponsePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                Key = reader.ReadString();
                int valueLength = reader.ReadVarInt();
                if (valueLength >= 0)
                {
                    Value = reader.ReadBuffer(valueLength);
                }
                else
                {
                    Value = null;
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CookieResponsePacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}