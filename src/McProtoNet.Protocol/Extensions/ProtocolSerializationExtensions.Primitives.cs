using System;
using System.Text;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    extension(MinecraftPrimitiveWriter writer)
    {
        public void WriteBuffer(ReadOnlySpan<byte> buff, int length)
        {
            if ((uint)length > (uint)buff.Length)
            {
                ThrowHelper.ThrowBufferLengthOutOfRange(length, buff.Length);
            }
            writer.WriteBuffer(buff[..length]);
        }

        public void WriteBuffer<TLen>(ReadOnlySpan<byte> buff)
        {
            if (typeof(TLen) == typeof(VarInt))
            {
                writer.WriteVarInt(buff.Length);
                writer.WriteBuffer(buff);
            }
        }

        // ── buffer with LengthFormat ──────────────────────────────────────────────
        /// <summary>Writes length (using <paramref name="lengthFormat"/>) then raw bytes.</summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void WriteBuffer(ReadOnlySpan<byte> buff, [System.Diagnostics.CodeAnalysis.ConstantExpected] LengthFormat lengthFormat = LengthFormat.VarInt)
        {
            WriteLength(writer, lengthFormat, buff.Length);
            writer.WriteBuffer(buff);
        }

        // ── pstring with LengthFormat ─────────────────────────────────────────────
        /// <summary>
        /// Writes a string with the specified length prefix format (for protodef <c>pstring</c>
        /// with non-VarInt CountType). Length is byte count of the UTF-8 encoded string.
        /// </summary>
        public void WriteString(string value, [System.Diagnostics.CodeAnalysis.ConstantExpected] LengthFormat lengthFormat)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            WriteLength(writer, lengthFormat, bytes.Length);
            writer.WriteBuffer(bytes);
        }

        // ── topBitSetTerminatedArray ──────────────────────────────────────────────
        /// <summary>
        /// Writes a byte array terminated by the "top bit not set" convention.
        /// For each element except the last, bit 7 is set (0x80 flag); for the last element bit 7 is 0.
        /// Values are masked to 7 bits (<c>value &amp; 0x7F</c>) before writing.
        /// </summary>
        public void WriteTopBitSetTerminatedArray(ReadOnlySpan<byte> values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                byte b = (byte)(values[i] & 0x7F);
                if (i < values.Length - 1) b |= 0x80; // signal "more follows"
                writer.WriteUnsignedByte(b);
            }
        }

        // ── loop (VarInt sentinel) ────────────────────────────────────────────────
        /// <summary>
        /// Writes an array of VarInts followed by the sentinel <paramref name="endValue"/>.
        /// Corresponds to protodef <c>loop</c> with VarInt element type.
        /// </summary>
        public void WriteLoopVarInt(ReadOnlySpan<int> values, int endValue)
        {
            foreach (int v in values)
                writer.WriteVarInt(v);
            writer.WriteVarInt(endValue);
        }
    }

    extension(ref MinecraftPrimitiveReader reader)
    {
        // ── buffer with LengthFormat ──────────────────────────────────────────────
        /// <summary>Reads length (using <paramref name="lengthFormat"/>) then reads that many bytes.</summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public byte[] ReadBuffer([System.Diagnostics.CodeAnalysis.ConstantExpected] LengthFormat lengthFormat = LengthFormat.VarInt)
        {
            int length = ReadLength(ref reader, lengthFormat);
            return reader.ReadBuffer(length);
        }

        // ── pstring with LengthFormat ─────────────────────────────────────────────
        /// <summary>
        /// Reads a string with the specified length prefix format (for protodef <c>pstring</c>
        /// with non-VarInt CountType). Length is byte count of the UTF-8 encoded string.
        /// </summary>
        public string ReadString([System.Diagnostics.CodeAnalysis.ConstantExpected] LengthFormat lengthFormat)
        {
            int byteCount = ReadLength(ref reader, lengthFormat);
            byte[] bytes = reader.ReadBuffer(byteCount);
            return Encoding.UTF8.GetString(bytes);
        }

        // ── topBitSetTerminatedArray ──────────────────────────────────────────────
        /// <summary>
        /// Reads bytes where bit 7 = 1 means "more elements follow" and bit 7 = 0 means last element.
        /// Each element value is the lower 7 bits (<c>byte &amp; 0x7F</c>).
        /// Corresponds to protodef <c>topBitSetTerminatedArray</c> with u8 element type.
        /// </summary>
        public byte[] ReadTopBitSetTerminatedArray()
        {
            var list = new System.Collections.Generic.List<byte>();
            byte b;
            do
            {
                b = reader.ReadUnsignedByte();
                list.Add((byte)(b & 0x7F));
            } while ((b & 0x80) != 0);
            return list.ToArray();
        }

        // ── loop (VarInt sentinel) ────────────────────────────────────────────────
        /// <summary>
        /// Reads VarInt elements until the value equals <paramref name="endValue"/> (sentinel).
        /// The sentinel is consumed but not included in the result.
        /// Corresponds to protodef <c>loop</c> with VarInt element type.
        /// </summary>
        public int[] ReadLoopVarInt(int endValue)
        {
            var list = new System.Collections.Generic.List<int>();
            int v;
            while ((v = reader.ReadVarInt()) != endValue)
                list.Add(v);
            return list.ToArray();
        }
    }

    // ── Helper methods ────────────────────────────────────────────────────────────

    private static object ReadRegistryEntryHolderBoxed(ref MinecraftPrimitiveReader reader, Type valueType,
        int protocolVersion)
    {
        if (valueType == typeof(string)) return reader.ReadRegistryEntryHolder<string>(protocolVersion);
        if (valueType == typeof(int)) return reader.ReadRegistryEntryHolder<int>(protocolVersion);
        if (valueType == typeof(ArmorTrimMaterial)) return reader.ReadRegistryEntryHolder<ArmorTrimMaterial>(protocolVersion);
        if (valueType == typeof(ArmorTrimPattern)) return reader.ReadRegistryEntryHolder<ArmorTrimPattern>(protocolVersion);
        if (valueType == typeof(BannerPattern)) return reader.ReadRegistryEntryHolder<BannerPattern>(protocolVersion);
        if (valueType == typeof(EntityMetadataPaintingVariant))
            return reader.ReadRegistryEntryHolder<EntityMetadataPaintingVariant>(protocolVersion);
        if (valueType == typeof(EntityMetadataWolfVariant))
            return reader.ReadRegistryEntryHolder<EntityMetadataWolfVariant>(protocolVersion);
        if (valueType == typeof(InstrumentData)) return reader.ReadRegistryEntryHolder<InstrumentData>(protocolVersion);
        if (valueType == typeof(ItemSoundEvent)) return reader.ReadRegistryEntryHolder<ItemSoundEvent>(protocolVersion);
        if (valueType == typeof(JukeboxSongData)) return reader.ReadRegistryEntryHolder<JukeboxSongData>(protocolVersion);

        throw new NotSupportedException($"ReadType<RegistryEntryHolder<{valueType.Name}>> is not registered.");
    }

    private static void WriteRegistryEntryHolderBoxed(MinecraftPrimitiveWriter writer, Type valueType, object value,
        int protocolVersion)
    {
        if (valueType == typeof(string))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<string>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(int))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<int>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(ArmorTrimMaterial))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<ArmorTrimMaterial>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(ArmorTrimPattern))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<ArmorTrimPattern>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(BannerPattern))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<BannerPattern>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(EntityMetadataPaintingVariant))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<EntityMetadataPaintingVariant>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(EntityMetadataWolfVariant))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<EntityMetadataWolfVariant>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(InstrumentData))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<InstrumentData>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(ItemSoundEvent))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<ItemSoundEvent>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(JukeboxSongData))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<JukeboxSongData>)value, protocolVersion);
            return;
        }

        throw new NotSupportedException($"WriteType<RegistryEntryHolder<{valueType.Name}>> is not registered.");
    }

    private static int ReadLength(ref MinecraftPrimitiveReader reader, LengthFormat lengthFormat)
    {
        return lengthFormat switch
        {
            LengthFormat.Byte => reader.ReadUnsignedByte(),
            LengthFormat.Short => reader.ReadSignedShort(),
            LengthFormat.Int => reader.ReadSignedInt(),
            LengthFormat.VarInt => reader.ReadVarInt(),
            _ => throw new ArgumentOutOfRangeException(nameof(lengthFormat))
        };
    }

    private static void WriteLength(MinecraftPrimitiveWriter writer, LengthFormat lengthFormat, int length)
    {
        switch (lengthFormat)
        {
            case LengthFormat.Byte:
                writer.WriteUnsignedByte((byte)length);
                break;
            case LengthFormat.Short:
                writer.WriteSignedShort((short)length);
                break;
            case LengthFormat.Int:
                writer.WriteSignedInt(length);
                break;
            case LengthFormat.VarInt:
                writer.WriteVarInt(length);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lengthFormat));
        }
    }
}
