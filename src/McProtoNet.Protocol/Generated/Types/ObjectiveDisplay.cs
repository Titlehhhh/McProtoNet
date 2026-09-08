using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class ObjectiveDisplay : IProtocolType<ObjectiveDisplay>
{
    public string DisplayTextJson { get; }
    public NbtTag DisplayText { get; }
    public int Type { get; }
    public int? NumberFormat { get; }
    public NbtTag? Styling { get; }

    public ObjectiveDisplay(string displayTextJson, NbtTag displayText, int type, int? numberFormat, NbtTag? styling)
    {
        DisplayTextJson = displayTextJson;
        DisplayText = displayText;
        Type = type;
        NumberFormat = numberFormat;
        Styling = styling;
    }

    public static ObjectiveDisplay Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ObjectiveDisplay>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var displayTextJson = reader.ReadString();
            var type = reader.ReadVarInt();
            return new ObjectiveDisplay(displayTextJson, default!, type, default!, default!);
        }

        if (protocolVersion >= 765)
        {
            var displayText = reader.ReadNbtTag(false)!;
            var type = reader.ReadVarInt();
            int? numberFormat = null;
            if (reader.ReadBoolean())
                numberFormat = reader.ReadVarInt();
            NbtTag? styling = default;
            if (numberFormat == 1 || numberFormat == 2)
            {
                var stylingValue = reader.ReadNbtTag(false)!;
                styling = stylingValue;
            }

            return new ObjectiveDisplay(default!, displayText, type, numberFormat, styling);
        }

        throw new System.NotSupportedException($"ObjectiveDisplay has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ObjectiveDisplay>(protocolVersion);
        if (protocolVersion <= 764)
        {
            writer.WriteString(DisplayTextJson);
            writer.WriteVarInt(Type);
            return;
        }

        if (protocolVersion >= 765)
        {
            writer.WriteNbt(DisplayText);
            writer.WriteVarInt(Type);
            writer.WriteBoolean(NumberFormat is not null);
            if (NumberFormat is { } numberFormatValue)
                writer.WriteVarInt(numberFormatValue);
            if (NumberFormat == 1 || NumberFormat == 2)
            {
                writer.WriteNbt((Styling ?? throw new System.InvalidOperationException("Styling is required at this protocol version.")));
            }
            else if (Styling is not null)
            {
                throw new System.InvalidOperationException("Styling is set, but 'number_format' does not select it at this protocol version.");
            }

            return;
        }

        throw new System.NotSupportedException($"ObjectiveDisplay has no wire layout for protocol version {protocolVersion}.");
    }
}
