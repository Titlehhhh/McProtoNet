using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("Success", PacketState.Login, PacketDirection.Clientbound)]
public abstract partial class SuccessPacket : IServerPacket
{
    public string Username { get; set; }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    [PacketSubInfo(340, 578)]
    public sealed partial class V340_578 : SuccessPacket
    {
        public string Uuid { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Uuid = reader.ReadString();
            Username = reader.ReadString();
        }
    }

    [PacketSubInfo(709, 758)]
    public sealed partial class V709_758 : SuccessPacket
    {
        public Guid Uuid { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Uuid = reader.ReadUUID();
            Username = reader.ReadString();
        }
    }

    [PacketSubInfo(759, 765)]
    public sealed partial class V759_765 : SuccessPacket
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
    }

    [PacketSubInfo(766, 767)]
    public sealed partial class V766_767 : SuccessPacket
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
    }

    [PacketSubInfo(768, 769)]
    public sealed partial class V768_769 : SuccessPacket
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
    }

    public class Property
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
        public string? Signature { get; set; }
    }
}