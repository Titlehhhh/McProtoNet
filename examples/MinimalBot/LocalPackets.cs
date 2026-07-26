// LocalPackets.cs — пакеты, определённые ПРЯМО в MinimalBot, снаружи библиотеки.
// Демонстрация инварианта дизайна: любой пакет собирается на ядре
// (IProtocolType<TSelf> + MinecraftPrimitiveWriter/Reader), без правок McProtoNet.

using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace MinimalBot.Packets;

/// <summary>
/// Serverbound чат-сообщение (pv 771–772, id 0x08), схема mcproto-facts «last».
/// Подпись не шлём — offline-сервер.
/// </summary>
public readonly record struct ChatMessagePacket(string Message, long Timestamp, long Salt)
    : IProtocolType<ChatMessagePacket>
{
    public static ChatMessagePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => throw new NotSupportedException("ChatMessagePacket: serverbound-only, Read не реализован");

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ReadOnlySpan<byte> acknowledged = [0, 0, 0]; // BitSet(20) фиксированной длины, все нули

        writer.WriteString(Message);       // message: varint length + utf8
        writer.WriteSignedLong(Timestamp); // timestamp: i64, Unix millis
        writer.WriteSignedLong(Salt);      // salt: i64
        writer.WriteBoolean(false);        // signature: option<256 bytes> — отсутствует
        writer.WriteVarInt(0);             // offset: varint
        writer.WriteBuffer(acknowledged);  // acknowledged: 3 сырых байта БЕЗ префикса длины
        writer.WriteUnsignedByte(0);       // checksum: u8
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x08;
        throw new NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}

/// <summary>
/// Serverbound «махнуть рукой» (pv 772, id 0x3C). Hand: 0 = основная рука.
/// </summary>
public readonly record struct ArmAnimationPacket(int Hand) : IProtocolType<ArmAnimationPacket>
{
    public static ArmAnimationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => new(reader.ReadVarInt());

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(Hand);

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion == 772)
            return 0x3C;
        throw new NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
