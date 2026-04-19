using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("ChatCommandSigned", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(766, 767, 0x05)]
[PacketId(768, 770, 0x06)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x07)]
public sealed partial class ChatCommandSignedPacket : IClientPacket
{
    public string Command { get; set; }
    public long Timestamp { get; set; }
    public long Salt { get; set; }
    public ArgumentSignature[] ArgumentSignatures { get; set; }
    public int MessageCount { get; set; }
    public byte[] Acknowledged { get; set; }
    public byte? Checksum { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 769:
                writer.WriteString(Command);
                writer.WriteSignedLong(Timestamp);
                writer.WriteSignedLong(Salt);
                writer.WriteVarInt(ArgumentSignatures.Length);
                foreach (var sig in ArgumentSignatures)
                {
                    writer.WriteString(sig.ArgumentName);
                    writer.WriteBuffer(sig.Signature);
                }
                writer.WriteVarInt(MessageCount);
                writer.WriteBuffer(Acknowledged);
                break;
            case >= 770 and <= MinecraftVersion.LatestProtocol:
                writer.WriteString(Command);
                writer.WriteSignedLong(Timestamp);
                writer.WriteSignedLong(Salt);
                writer.WriteVarInt(ArgumentSignatures.Length);
                foreach (var sig in ArgumentSignatures)
                {
                    writer.WriteString(sig.ArgumentName);
                    writer.WriteBuffer(sig.Signature);
                }
                writer.WriteVarInt(MessageCount);
                writer.WriteBuffer(Acknowledged);
                writer.WriteSignedByte(Checksum ?? throw new InvalidOperationException("ChatCommandSignedPacket checksum missing."));
                break;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ChatCommandSignedPacket), protocolVersion, SupportedVersions);
                break;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 769:
                Command = reader.ReadString();
                Timestamp = reader.ReadSignedLong();
                Salt = reader.ReadSignedLong();
                {
                    int count = reader.ReadVarInt();
                    var list = new ArgumentSignature[count];
                    for (int i = 0; i < count; i++)
                    {
                        var argName = reader.ReadString();
                        var signature = reader.ReadBuffer(256);
                        list[i] = new ArgumentSignature { ArgumentName = argName, Signature = signature };
                    }
                    ArgumentSignatures = list;
                }
                MessageCount = reader.ReadVarInt();
                Acknowledged = reader.ReadBuffer(3);
                break;
            case >= 770 and <= MinecraftVersion.LatestProtocol:
                Command = reader.ReadString();
                Timestamp = reader.ReadSignedLong();
                Salt = reader.ReadSignedLong();
                {
                    int count = reader.ReadVarInt();
                    var list = new ArgumentSignature[count];
                    for (int i = 0; i < count; i++)
                    {
                        var argName = reader.ReadString();
                        var signature = reader.ReadBuffer(256);
                        list[i] = new ArgumentSignature { ArgumentName = argName, Signature = signature };
                    }
                    ArgumentSignatures = list;
                }
                MessageCount = reader.ReadVarInt();
                Acknowledged = reader.ReadBuffer(3);
                Checksum = reader.ReadSignedByte();
                break;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ChatCommandSignedPacket), protocolVersion, SupportedVersions);
                break;
        }
    }

    public struct ArgumentSignature
    {
        public string ArgumentName { get; set; }
        public byte[] Signature { get; set; }
    }
}