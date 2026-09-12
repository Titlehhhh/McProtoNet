using Dunet;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using McProtoNet.NBT;

namespace McProtoNet.Protocol;

[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
[Union]
public partial record ServerLinkLabel
{
    partial record KnownType(ServerLinkType Type);
    partial record Custom(NbtTag Label);
    public static ServerLinkLabel Read(ref MinecraftPrimitiveReader reader, int protocolVersion, int discriminator)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerLinkLabel>(protocolVersion);
        switch (discriminator)
        {
            case 1:
            {
                var type = reader.ReadType<ServerLinkType>(protocolVersion);
                return new KnownType(type);
            }

            case 0:
            {
                var label = reader.ReadNbtTag(false)!;
                return new Custom(label);
            }
        }

        throw new System.NotSupportedException($"ServerLinkLabel has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerLinkLabel>(protocolVersion);
        switch (this)
        {
            case KnownType arm:
            {
                ServerLinkType Type = arm.Type;
                writer.WriteType<ServerLinkType>(Type, protocolVersion);
                return;
            }

            case Custom arm:
            {
                NbtTag Label = arm.Label;
                writer.WriteNbt(Label);
                return;
            }
        }

        throw new System.NotSupportedException($"ServerLinkLabel case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }

    public int Discriminator(int protocolVersion)
    {
        switch (this)
        {
            case KnownType _:
                return 1;
            case Custom _:
                return 0;
        }

        throw new System.NotSupportedException($"ServerLinkLabel case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        switch (this)
        {
            case KnownType arm:
            {
                writer.WriteString("$case", "KnownType");
                writer.WritePropertyName("Type");
                writer.WriteStringValue(arm.Type.ToString());
                break;
            }

            case Custom arm:
            {
                writer.WriteString("$case", "Custom");
                writer.WritePropertyName("Label");
                arm.Label.WriteJson(writer);
                break;
            }

            default:
                throw new System.NotSupportedException($"ServerLinkLabel case {GetType().Name} has no JSON view.");
        }

        writer.WriteEndObject();
    }
}
