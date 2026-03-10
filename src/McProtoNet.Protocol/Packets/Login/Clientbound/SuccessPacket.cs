using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("Success", PacketState.Login, PacketDirection.Clientbound)]
public sealed partial class SuccessPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, 765),
        new(766, 767),
        new(768, MinecraftVersion.LatestProtocol)
    };

    public string Username { get; set; } = string.Empty;

    public VFirst_758Fields? VFirst_758 { get; set; }
    public V759_765Fields? V759_765 { get; set; }
    public V766_767Fields? V766_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                var fields = VFirst_758 ?? throw new InvalidOperationException("Success VFirst_758 missing.");
                writer.WriteUUID(fields.Uuid);
                writer.WriteString(Username);
                return;
            }
            case >= 759 and <= 765:
            {
                var fields = V759_765 ?? throw new InvalidOperationException("Success V759_765 missing.");
                writer.WriteUUID(fields.Uuid);
                writer.WriteString(Username);
                var properties = fields.Properties ?? Array.Empty<Property>();
                writer.WriteVarInt(properties.Length);
                for (int i = 0; i < properties.Length; i++)
                {
                    writer.WriteString(properties[i].Name ?? string.Empty);
                    writer.WriteString(properties[i].Value ?? string.Empty);
                    if (properties[i].Signature is null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteString(properties[i].Signature);
                    }
                }
                return;
            }
            case >= 766 and <= 767:
            {
                var fields = V766_767 ?? throw new InvalidOperationException("Success V766_767 missing.");
                writer.WriteUUID(fields.Uuid);
                writer.WriteString(Username);
                var properties = fields.Properties ?? Array.Empty<Property>();
                writer.WriteVarInt(properties.Length);
                for (int i = 0; i < properties.Length; i++)
                {
                    writer.WriteString(properties[i].Name ?? string.Empty);
                    writer.WriteString(properties[i].Value ?? string.Empty);
                    if (properties[i].Signature is null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteString(properties[i].Signature);
                    }
                }
                writer.WriteBoolean(fields.StrictErrorHandling);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("Success V768_Last missing.");
                writer.WriteUUID(fields.Uuid);
                writer.WriteString(Username);
                var properties = fields.Properties ?? Array.Empty<Property>();
                writer.WriteVarInt(properties.Length);
                for (int i = 0; i < properties.Length; i++)
                {
                    writer.WriteString(properties[i].Name ?? string.Empty);
                    writer.WriteString(properties[i].Value ?? string.Empty);
                    if (properties[i].Signature is null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteString(properties[i].Signature);
                    }
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerLoginPacket.Success), protocolVersion, SupportedVersionsStatic);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                VFirst_758 = new VFirst_758Fields
                {
                    Uuid = reader.ReadUUID()
                };
                Username = reader.ReadString();
                return;
            case >= 759 and <= 765:
            {
                Guid uuid = reader.ReadUUID();
                Username = reader.ReadString();
                int count = reader.ReadVarInt();
                var properties = count == 0 ? Array.Empty<Property>() : new Property[count];
                for (int i = 0; i < properties.Length; i++)
                {
                    string name = reader.ReadString();
                    string value = reader.ReadString();
                    bool hasSignature = reader.ReadBoolean();
                    string? signature = hasSignature ? reader.ReadString() : null;
                    properties[i] = new Property
                    {
                        Name = name,
                        Value = value,
                        Signature = signature
                    };
                }
                V759_765 = new V759_765Fields
                {
                    Uuid = uuid,
                    Properties = properties
                };
                return;
            }
            case >= 766 and <= 767:
            {
                Guid uuid = reader.ReadUUID();
                Username = reader.ReadString();
                int count = reader.ReadVarInt();
                var properties = count == 0 ? Array.Empty<Property>() : new Property[count];
                for (int i = 0; i < properties.Length; i++)
                {
                    string name = reader.ReadString();
                    string value = reader.ReadString();
                    bool hasSignature = reader.ReadBoolean();
                    string? signature = hasSignature ? reader.ReadString() : null;
                    properties[i] = new Property
                    {
                        Name = name,
                        Value = value,
                        Signature = signature
                    };
                }
                V766_767 = new V766_767Fields
                {
                    Uuid = uuid,
                    Properties = properties,
                    StrictErrorHandling = reader.ReadBoolean()
                };
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                Guid uuid = reader.ReadUUID();
                Username = reader.ReadString();
                int count = reader.ReadVarInt();
                var properties = count == 0 ? Array.Empty<Property>() : new Property[count];
                for (int i = 0; i < properties.Length; i++)
                {
                    string name = reader.ReadString();
                    string value = reader.ReadString();
                    bool hasSignature = reader.ReadBoolean();
                    string? signature = hasSignature ? reader.ReadString() : null;
                    properties[i] = new Property
                    {
                        Name = name,
                        Value = value,
                        Signature = signature
                    };
                }
                V768_Last = new V768_LastFields
                {
                    Uuid = uuid,
                    Properties = properties
                };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerLoginPacket.Success), protocolVersion, SupportedVersionsStatic);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_758Fields
    {
        public Guid Uuid { get; set; }
    }

    public struct V759_765Fields
    {
        public Guid Uuid { get; set; }
        public Property[]? Properties { get; set; }
    }

    public struct V766_767Fields
    {
        public Guid Uuid { get; set; }
        public Property[]? Properties { get; set; }
        public bool StrictErrorHandling { get; set; }
    }

    public struct V768_LastFields
    {
        public Guid Uuid { get; set; }
        public Property[]? Properties { get; set; }
    }

    public class Property
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
        public string? Signature { get; set; }
    }
}