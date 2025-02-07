using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("BlockPlace", PacketState.Play, PacketDirection.Serverbound)]
    public partial class BlockPlacePacket : IClientPacket
    {
        public Position Location { get; set; }
        public int Direction { get; set; }
        public int Hand { get; set; }
        public float CursorX { get; set; }
        public float CursorY { get; set; }
        public float CursorZ { get; set; }

        [PacketSubInfo(340, 404)]
        public sealed partial class V340_404 : BlockPlacePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Location, Direction, Hand, CursorX, CursorY, CursorZ);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                Position location, int direction, int hand, float cursorX, float cursorY, float cursorZ)
            {
                writer.WritePosition(location, protocolVersion);
                writer.WriteVarInt(direction);
                writer.WriteVarInt(hand);
                writer.WriteFloat(cursorX);
                writer.WriteFloat(cursorY);
                writer.WriteFloat(cursorZ);
            }
        }

        [PacketSubInfo(477, 758)]
        public sealed partial class V477_758 : BlockPlacePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Hand, Location, Direction, CursorX, CursorY, CursorZ,
                    InsideBlock);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int hand,
                Position location, int direction, float cursorX, float cursorY, float cursorZ, bool insideBlock)
            {
                writer.WriteVarInt(hand);
                writer.WritePosition(location, protocolVersion);
                writer.WriteVarInt(direction);
                writer.WriteFloat(cursorX);
                writer.WriteFloat(cursorY);
                writer.WriteFloat(cursorZ);
                writer.WriteBoolean(insideBlock);
            }

            public bool InsideBlock { get; set; }
        }

        [PacketSubInfo(759, 767)]
        public sealed partial class V759_767 : BlockPlacePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Hand, Location, Direction, CursorX, CursorY, CursorZ,
                    InsideBlock, Sequence);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int hand,
                Position location, int direction, float cursorX, float cursorY, float cursorZ, bool insideBlock,
                int sequence)
            {
                writer.WriteVarInt(hand);
                writer.WritePosition(location, protocolVersion);
                writer.WriteVarInt(direction);
                writer.WriteFloat(cursorX);
                writer.WriteFloat(cursorY);
                writer.WriteFloat(cursorZ);
                writer.WriteBoolean(insideBlock);
                writer.WriteVarInt(sequence);
            }

            public bool InsideBlock { get; set; }
            public int Sequence { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : BlockPlacePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Hand, Location, Direction, CursorX, CursorY, CursorZ,
                    InsideBlock, WorldBorderHit, Sequence);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int hand,
                Position location, int direction, float cursorX, float cursorY, float cursorZ, bool insideBlock,
                bool worldBorderHit, int sequence)
            {
                writer.WriteVarInt(hand);
                writer.WritePosition(location, protocolVersion);
                writer.WriteVarInt(direction);
                writer.WriteFloat(cursorX);
                writer.WriteFloat(cursorY);
                writer.WriteFloat(cursorZ);
                writer.WriteBoolean(insideBlock);
                writer.WriteBoolean(worldBorderHit);
                writer.WriteVarInt(sequence);
            }

            public bool InsideBlock { get; set; }
            public bool WorldBorderHit { get; set; }
            public int Sequence { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_404.IsSupportedVersionStatic(protocolVersion))
                V340_404.SerializeInternal(ref writer, protocolVersion, Location, Direction, Hand, CursorX, CursorY,
                    CursorZ);
            else if (V477_758.IsSupportedVersionStatic(protocolVersion))
                V477_758.SerializeInternal(ref writer, protocolVersion, Hand, Location, Direction, CursorX, CursorY,
                    CursorZ, false);
            else if (V759_767.IsSupportedVersionStatic(protocolVersion))
                V759_767.SerializeInternal(ref writer, protocolVersion, Hand, Location, Direction, CursorX, CursorY,
                    CursorZ, false, 0);
            else if (V768_769.IsSupportedVersionStatic(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, Hand, Location, Direction, CursorX, CursorY,
                    CursorZ, false, false, 0);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.BlockPlace), protocolVersion);
        }
    }
}