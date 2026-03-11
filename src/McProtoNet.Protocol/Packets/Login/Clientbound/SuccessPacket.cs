using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("Success", PacketState.Login, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol, 0x02)]
public sealed partial class SuccessPacket : IServerPacket
{
    public Guid Uuid { get; set; }
    public string Username { get; set; }

    public V759_765Fields? V759_765 { get; set; }
    public V766_767Fields? V766_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                writer.WriteUUID(Uuid);
                writer.WriteString(Username);
                return;
            case >= 759 and <= 765:
            {
                var fields = V759_765 ?? throw new InvalidOperationException("SuccessPacket 759-765 fields missing.");
                writer.WriteUUID(Uuid);
                writer.WriteString(Username);
                writer.WriteVarInt(fields.Properties.Length);
                foreach (var p in fields.Properties) { writer.WriteString(p.Name); writer.WriteString(p.Value); writer.WriteString(p.Signature); }
                return;
            }
            case >= 766 and <= 767:
            {
                var fields = V766_767 ?? throw new InvalidOperationException("SuccessPacket 766-767 fields missing.");
                writer.WriteUUID(Uuid);
                writer.WriteString(Username);
                writer.WriteVarInt(fields.Properties.Length);
                foreach (var p in fields.Properties) { writer.WriteString(p.Name); writer.WriteString(p.Value); writer.WriteString(p.Signature); }
                writer.WriteBoolean(fields.StrictErrorHandling);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("SuccessPacket 768-last fields missing.");
                writer.WriteUUID(Uuid);
                writer.WriteString(Username);
                writer.WriteVarInt(fields.Properties.Length);
                foreach (var p in fields.Properties) { writer.WriteString(p.Name); writer.WriteString(p.Value); writer.WriteString(p.Signature); }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SuccessPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                Uuid = reader.ReadUUID();
                Username = reader.ReadString();
                return;
            case >= 759 and <= 765:
            {
                Uuid = reader.ReadUUID();
                Username = reader.ReadString();
                var c759 = reader.ReadVarInt();
                var p759 = new Property[c759];
                for (int i = 0; i < c759; i++) p759[i] = new Property { Name = reader.ReadString(), Value = reader.ReadString(), Signature = reader.ReadString() };
                V759_765 = new V759_765Fields { Properties = p759 };
                return;
            }
            case >= 766 and <= 767:
            {
                Uuid = reader.ReadUUID();
                Username = reader.ReadString();
                var c766 = reader.ReadVarInt();
                var p766 = new Property[c766];
                for (int i = 0; i < c766; i++) p766[i] = new Property { Name = reader.ReadString(), Value = reader.ReadString(), Signature = reader.ReadString() };
                V766_767 = new V766_767Fields { Properties = p766, StrictErrorHandling = reader.ReadBoolean() };
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                Uuid = reader.ReadUUID();
                Username = reader.ReadString();
                var c768 = reader.ReadVarInt();
                var p768 = new Property[c768];
                for (int i = 0; i < c768; i++) p768[i] = new Property { Name = reader.ReadString(), Value = reader.ReadString(), Signature = reader.ReadString() };
                V768_Last = new V768_LastFields { Properties = p768 };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SuccessPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759_765Fields
    {
        public Property[] Properties { get; set; }
    }

    public struct V766_767Fields
    {
        public Property[] Properties { get; set; }
        public bool StrictErrorHandling { get; set; }
    }

    public struct V768_LastFields
    {
        public Property[] Properties { get; set; }
    }

    public struct Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Signature { get; set; }
    }
}