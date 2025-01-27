using McProtoNet.Serialization;

namespace McProtoNet.Protocol.ClientboundPackets.Login;

public abstract class PacketSuccess : IServerPacket
{
    public string Username { get; set; }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    public sealed class V340_578 : PacketSuccess
    {
        public string Uuid { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Uuid = reader.ReadString();
            Username = reader.ReadString();
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 340 and <= 578;
        }
    }

    public sealed class V709_758 : PacketSuccess
    {
        public Guid Uuid { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Uuid = reader.ReadUUID();
            Username = reader.ReadString();
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 709 and <= 758;
        }
    }

    public sealed class V759_765 : PacketSuccess
    {
        public Property[] Properties { get; set; }
        public Guid Uuid { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Uuid = reader.ReadUUID();
            Username = reader.ReadString();
            var count = reader.ReadVarInt();
            Properties = new Property[count];
            for (int i = 0; i < count; i++)
            {
                Properties[i] = new Property
                {
                    Name = reader.ReadString(),
                    Value = reader.ReadString(),
                    Signature = reader.ReadOptional(ReadDelegates.String)
                };
            }
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 759 and <= 765;
        }
    }

    public sealed class V766_767 : PacketSuccess
    {
        public Property[] Properties { get; set; }
        public bool StrictErrorHandling { get; set; }
        public Guid Uuid { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Uuid = reader.ReadUUID();
            Username = reader.ReadString();
            var count = reader.ReadVarInt();
            Properties = new Property[count];
            for (int i = 0; i < count; i++)
            {
                Properties[i] = new Property
                {
                    Name = reader.ReadString(),
                    Value = reader.ReadString(),
                    Signature = reader.ReadOptional(ReadDelegates.String)
                };
            }

            StrictErrorHandling = reader.ReadBoolean();
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 766 and <= 767;
        }
    }

    public sealed class V768_769 : PacketSuccess
    {
        public Guid Uuid { get; set; }
        public Property[] Properties { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Uuid = reader.ReadUUID();
            Username = reader.ReadString();
            var count = reader.ReadVarInt();
            Properties = new Property[count];
            for (int i = 0; i < count; i++)
            {
                Properties[i] = new Property
                {
                    Name = reader.ReadString(),
                    Value = reader.ReadString(),
                    Signature = reader.ReadOptional(ReadDelegates.String)
                };
            }
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 768 and <= 769;
        }
    }

    public class Property
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
        public string? Signature { get; set; }
    }


    public static bool SupportedVersion(int protocolVersion)
    {
        return V340_578.SupportedVersion(protocolVersion);
    }

    public static PacketIdentifier PacketId => ServerLoginPacket.Success;

    public PacketIdentifier GetPacketId() => PacketId;
}