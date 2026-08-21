using McProtoNet.Primitives;
using McProtoNet.Protocol;

namespace MinimalBot.Packets;

/// <summary>
///     Пакет, определённый снаружи библиотеки: serverbound «махнуть рукой», Hand 0 — основная рука.
///     Любой пакет собирается на ядре (<see cref="IProtocolType{TSelf}" /> плюс writer/reader),
///     правки McProtoNet не нужны.
/// </summary>
public readonly record struct ArmAnimationPacket(int Hand) : IProtocolType<ArmAnimationPacket>
{
    public static ArmAnimationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => new(reader.ReadVarInt());

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(Hand);

    /// <summary>
    ///     Номер пакета на версии протокола; своей регистрации в реестре у него нет. Диапазон —
    ///     из McProtoFacts: <c>ids play.toServer.arm_animation</c> даёт 0x3C на 771–774.
    /// </summary>
    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion is >= 771 and <= 774) return 0x3C;
        throw new NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
