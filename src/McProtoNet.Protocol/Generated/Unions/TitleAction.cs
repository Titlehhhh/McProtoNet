using Dunet;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[Union]
public partial record TitleAction
{
    partial record SetTitle(string TextJson);
    partial record SetSubtitle(string TextJson);
    partial record SetActionBar(string TextJson);
    partial record SetTimes(int FadeIn, int Stay, int FadeOut);
    public static TitleAction Read(ref MinecraftPrimitiveReader reader, int protocolVersion, int discriminator)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TitleAction>(protocolVersion);
        switch (discriminator)
        {
            case 0:
            {
                var textJson = reader.ReadString();
                return new SetTitle(textJson);
            }

            case 1:
            {
                var textJson = reader.ReadString();
                return new SetSubtitle(textJson);
            }

            case 2:
            {
                var textJson = reader.ReadString();
                return new SetActionBar(textJson);
            }

            case 3:
            {
                var fadeIn = reader.ReadSignedInt();
                var stay = reader.ReadSignedInt();
                var fadeOut = reader.ReadSignedInt();
                return new SetTimes(fadeIn, stay, fadeOut);
            }
        }

        throw new System.NotSupportedException($"TitleAction has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TitleAction>(protocolVersion);
        switch (this)
        {
            case SetTitle arm:
            {
                string TextJson = arm.TextJson;
                writer.WriteString(TextJson);
                return;
            }

            case SetSubtitle arm:
            {
                string TextJson = arm.TextJson;
                writer.WriteString(TextJson);
                return;
            }

            case SetActionBar arm:
            {
                string TextJson = arm.TextJson;
                writer.WriteString(TextJson);
                return;
            }

            case SetTimes arm:
            {
                int FadeIn = arm.FadeIn;
                int Stay = arm.Stay;
                int FadeOut = arm.FadeOut;
                writer.WriteSignedInt(FadeIn);
                writer.WriteSignedInt(Stay);
                writer.WriteSignedInt(FadeOut);
                return;
            }
        }

        throw new System.NotSupportedException($"TitleAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }

    public int Discriminator(int protocolVersion)
    {
        switch (this)
        {
            case SetTitle _:
                return 0;
            case SetSubtitle _:
                return 1;
            case SetActionBar _:
                return 2;
            case SetTimes _:
                return 3;
        }

        throw new System.NotSupportedException($"TitleAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }
}
