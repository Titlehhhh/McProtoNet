using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("ServerData", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ServerDataPacket : IServerPacket
    {
        [PacketSubInfo(759, 759)]
        public sealed partial class V759 : ServerDataPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Motd = reader.ReadOptional(ReadDelegates.String);
                Icon = reader.ReadOptional(ReadDelegates.String);
                PreviewsChat = reader.ReadBoolean();
            }

            public string? Motd { get; set; }
            public string? Icon { get; set; }
            public bool PreviewsChat { get; set; }
        }

        [PacketSubInfo(760, 760)]
        public sealed partial class V760 : ServerDataPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Motd = reader.ReadOptional(ReadDelegates.String);
                Icon = reader.ReadOptional(ReadDelegates.String);
                PreviewsChat = reader.ReadBoolean();
                EnforcesSecureChat = reader.ReadBoolean();
            }

            public string? Motd { get; set; }
            public string? Icon { get; set; }
            public bool PreviewsChat { get; set; }
            public bool EnforcesSecureChat { get; set; }
        }

        [PacketSubInfo(761, 761)]
        public sealed partial class V761 : ServerDataPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Motd = reader.ReadOptional(ReadDelegates.String);
                Icon = reader.ReadOptional(ReadDelegates.String);
                EnforcesSecureChat = reader.ReadBoolean();
            }

            public string? Motd { get; set; }
            public string? Icon { get; set; }
            public bool EnforcesSecureChat { get; set; }
        }

        [PacketSubInfo(762, 764)]
        public sealed partial class V762_764 : ServerDataPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Motd = reader.ReadString();
                IconBytes = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) =>
                    r_0.ReadBuffer(LengthFormat.VarInt));
                EnforcesSecureChat = reader.ReadBoolean();
            }

            public string Motd { get; set; }
            public byte[]? IconBytes { get; set; }
            public bool EnforcesSecureChat { get; set; }
        }

        [PacketSubInfo(765, 765)]
        public sealed partial class V765 : ServerDataPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Motd = reader.ReadNbtTag(false);
                IconBytes = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) =>
                    r_0.ReadBuffer(LengthFormat.VarInt));
                EnforcesSecureChat = reader.ReadBoolean();
            }

            public NbtTag Motd { get; set; }
            public byte[]? IconBytes { get; set; }
            public bool EnforcesSecureChat { get; set; }
        }

        [PacketSubInfo(766, 769)]
        public sealed partial class V766_769 : ServerDataPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Motd = reader.ReadNbtTag(false);
                IconBytes = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) =>
                    r_0.ReadBuffer(LengthFormat.VarInt));
            }

            public NbtTag Motd { get; set; }
            public byte[]? IconBytes { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}