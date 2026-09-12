using Dunet;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using McProtoNet.NBT;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Union]
public partial record BossBarAction
{
    partial record AddVUntil764(string Title, float Health, int Color, int Dividers, byte Flags);
    partial record Remove();
    partial record UpdateHealth(float Health);
    partial record UpdateTitleVUntil764(string Title);
    partial record UpdateStyle(int Color, int Dividers);
    partial record UpdateFlags(byte Flags);
    partial record AddV765_Last(NbtTag Title, float Health, int Color, int Dividers, byte Flags);
    partial record UpdateTitleV765_Last(NbtTag Title);
    public static BossBarAction Read(ref MinecraftPrimitiveReader reader, int protocolVersion, int discriminator)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BossBarAction>(protocolVersion);
        if (protocolVersion <= 764)
        {
            switch (discriminator)
            {
                case 0:
                {
                    var title = reader.ReadString();
                    var health = reader.ReadFloat();
                    var color = reader.ReadVarInt();
                    var dividers = reader.ReadVarInt();
                    var flags = reader.ReadUnsignedByte();
                    return new AddVUntil764(title, health, color, dividers, flags);
                }

                case 1:
                {
                    return new Remove();
                }

                case 2:
                {
                    var health = reader.ReadFloat();
                    return new UpdateHealth(health);
                }

                case 3:
                {
                    var title = reader.ReadString();
                    return new UpdateTitleVUntil764(title);
                }

                case 4:
                {
                    var color = reader.ReadVarInt();
                    var dividers = reader.ReadVarInt();
                    return new UpdateStyle(color, dividers);
                }

                case 5:
                {
                    var flags = reader.ReadUnsignedByte();
                    return new UpdateFlags(flags);
                }
            }

            throw new System.NotSupportedException($"BossBarAction has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
        }

        if (protocolVersion >= 765)
        {
            switch (discriminator)
            {
                case 0:
                {
                    var title = reader.ReadNbtTag(false)!;
                    var health = reader.ReadFloat();
                    var color = reader.ReadVarInt();
                    var dividers = reader.ReadVarInt();
                    var flags = reader.ReadUnsignedByte();
                    return new AddV765_Last(title, health, color, dividers, flags);
                }

                case 1:
                {
                    return new Remove();
                }

                case 2:
                {
                    var health = reader.ReadFloat();
                    return new UpdateHealth(health);
                }

                case 3:
                {
                    var title = reader.ReadNbtTag(false)!;
                    return new UpdateTitleV765_Last(title);
                }

                case 4:
                {
                    var color = reader.ReadVarInt();
                    var dividers = reader.ReadVarInt();
                    return new UpdateStyle(color, dividers);
                }

                case 5:
                {
                    var flags = reader.ReadUnsignedByte();
                    return new UpdateFlags(flags);
                }
            }

            throw new System.NotSupportedException($"BossBarAction has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
        }

        throw new System.NotSupportedException($"BossBarAction has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BossBarAction>(protocolVersion);
        if (protocolVersion <= 764)
        {
            switch (this)
            {
                case AddVUntil764 arm:
                {
                    string Title = arm.Title;
                    float Health = arm.Health;
                    int Color = arm.Color;
                    int Dividers = arm.Dividers;
                    byte Flags = arm.Flags;
                    writer.WriteString(Title);
                    writer.WriteFloat(Health);
                    writer.WriteVarInt(Color);
                    writer.WriteVarInt(Dividers);
                    writer.WriteUnsignedByte(Flags);
                    return;
                }

                case Remove _:
                {
                    return;
                }

                case UpdateHealth arm:
                {
                    float Health = arm.Health;
                    writer.WriteFloat(Health);
                    return;
                }

                case UpdateTitleVUntil764 arm:
                {
                    string Title = arm.Title;
                    writer.WriteString(Title);
                    return;
                }

                case UpdateStyle arm:
                {
                    int Color = arm.Color;
                    int Dividers = arm.Dividers;
                    writer.WriteVarInt(Color);
                    writer.WriteVarInt(Dividers);
                    return;
                }

                case UpdateFlags arm:
                {
                    byte Flags = arm.Flags;
                    writer.WriteUnsignedByte(Flags);
                    return;
                }
            }

            throw new System.NotSupportedException($"BossBarAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
        }

        if (protocolVersion >= 765)
        {
            switch (this)
            {
                case AddV765_Last arm:
                {
                    NbtTag Title = arm.Title;
                    float Health = arm.Health;
                    int Color = arm.Color;
                    int Dividers = arm.Dividers;
                    byte Flags = arm.Flags;
                    writer.WriteNbt(Title);
                    writer.WriteFloat(Health);
                    writer.WriteVarInt(Color);
                    writer.WriteVarInt(Dividers);
                    writer.WriteUnsignedByte(Flags);
                    return;
                }

                case Remove _:
                {
                    return;
                }

                case UpdateHealth arm:
                {
                    float Health = arm.Health;
                    writer.WriteFloat(Health);
                    return;
                }

                case UpdateTitleV765_Last arm:
                {
                    NbtTag Title = arm.Title;
                    writer.WriteNbt(Title);
                    return;
                }

                case UpdateStyle arm:
                {
                    int Color = arm.Color;
                    int Dividers = arm.Dividers;
                    writer.WriteVarInt(Color);
                    writer.WriteVarInt(Dividers);
                    return;
                }

                case UpdateFlags arm:
                {
                    byte Flags = arm.Flags;
                    writer.WriteUnsignedByte(Flags);
                    return;
                }
            }

            throw new System.NotSupportedException($"BossBarAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
        }

        throw new System.NotSupportedException($"BossBarAction has no wire layout for protocol version {protocolVersion}.");
    }

    public int Discriminator(int protocolVersion)
    {
        if (protocolVersion <= 764)
        {
            switch (this)
            {
                case AddVUntil764 _:
                    return 0;
                case Remove _:
                    return 1;
                case UpdateHealth _:
                    return 2;
                case UpdateTitleVUntil764 _:
                    return 3;
                case UpdateStyle _:
                    return 4;
                case UpdateFlags _:
                    return 5;
            }

            throw new System.NotSupportedException($"BossBarAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
        }

        if (protocolVersion >= 765)
        {
            switch (this)
            {
                case AddV765_Last _:
                    return 0;
                case Remove _:
                    return 1;
                case UpdateHealth _:
                    return 2;
                case UpdateTitleV765_Last _:
                    return 3;
                case UpdateStyle _:
                    return 4;
                case UpdateFlags _:
                    return 5;
            }

            throw new System.NotSupportedException($"BossBarAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
        }

        throw new System.NotSupportedException($"BossBarAction has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        switch (this)
        {
            case AddVUntil764 arm:
            {
                writer.WriteString("$case", "Add");
                writer.WritePropertyName("Title");
                writer.WriteStringValue(arm.Title);
                writer.WritePropertyName("Health");
                if (double.IsFinite(arm.Health))
                    writer.WriteNumberValue(arm.Health);
                else
                    writer.WriteStringValue(double.IsNaN(arm.Health) ? "NaN" : arm.Health > 0 ? "Infinity" : "-Infinity");
                writer.WritePropertyName("Color");
                writer.WriteNumberValue(arm.Color);
                writer.WritePropertyName("Dividers");
                writer.WriteNumberValue(arm.Dividers);
                writer.WritePropertyName("Flags");
                writer.WriteNumberValue(arm.Flags);
                break;
            }

            case Remove _:
            {
                writer.WriteString("$case", "Remove");
                break;
            }

            case UpdateHealth arm:
            {
                writer.WriteString("$case", "UpdateHealth");
                writer.WritePropertyName("Health");
                if (double.IsFinite(arm.Health))
                    writer.WriteNumberValue(arm.Health);
                else
                    writer.WriteStringValue(double.IsNaN(arm.Health) ? "NaN" : arm.Health > 0 ? "Infinity" : "-Infinity");
                break;
            }

            case UpdateTitleVUntil764 arm:
            {
                writer.WriteString("$case", "UpdateTitle");
                writer.WritePropertyName("Title");
                writer.WriteStringValue(arm.Title);
                break;
            }

            case UpdateStyle arm:
            {
                writer.WriteString("$case", "UpdateStyle");
                writer.WritePropertyName("Color");
                writer.WriteNumberValue(arm.Color);
                writer.WritePropertyName("Dividers");
                writer.WriteNumberValue(arm.Dividers);
                break;
            }

            case UpdateFlags arm:
            {
                writer.WriteString("$case", "UpdateFlags");
                writer.WritePropertyName("Flags");
                writer.WriteNumberValue(arm.Flags);
                break;
            }

            case AddV765_Last arm:
            {
                writer.WriteString("$case", "Add");
                writer.WritePropertyName("Title");
                arm.Title.WriteJson(writer);
                writer.WritePropertyName("Health");
                if (double.IsFinite(arm.Health))
                    writer.WriteNumberValue(arm.Health);
                else
                    writer.WriteStringValue(double.IsNaN(arm.Health) ? "NaN" : arm.Health > 0 ? "Infinity" : "-Infinity");
                writer.WritePropertyName("Color");
                writer.WriteNumberValue(arm.Color);
                writer.WritePropertyName("Dividers");
                writer.WriteNumberValue(arm.Dividers);
                writer.WritePropertyName("Flags");
                writer.WriteNumberValue(arm.Flags);
                break;
            }

            case UpdateTitleV765_Last arm:
            {
                writer.WriteString("$case", "UpdateTitle");
                writer.WritePropertyName("Title");
                arm.Title.WriteJson(writer);
                break;
            }

            default:
                throw new System.NotSupportedException($"BossBarAction case {GetType().Name} has no JSON view.");
        }

        writer.WriteEndObject();
    }
}
