using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class EntityAttribute : IProtocolType<EntityAttribute>
{
    public string KeyName { get; }
    public int KeyId { get; }
    public double Value { get; }
    public AttributeModifier[] Modifiers { get; }

    public EntityAttribute(string keyName, int keyId, double value, AttributeModifier[] modifiers)
    {
        KeyName = keyName;
        KeyId = keyId;
        Value = value;
        Modifiers = modifiers;
    }

    public static EntityAttribute Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityAttribute>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var keyName = reader.ReadString();
            var value = reader.ReadDouble();
            int modifiersCount = reader.ReadVarInt();
            var modifiers = new AttributeModifier[modifiersCount];
            for (int i = 0; i < modifiers.Length; i++)
                modifiers[i] = reader.ReadType<AttributeModifier>(protocolVersion);
            return new EntityAttribute(keyName, default!, value, modifiers);
        }

        if (protocolVersion >= 755 && protocolVersion <= 765)
        {
            var keyName = reader.ReadString();
            var value = reader.ReadDouble();
            int modifiersCount = reader.ReadVarInt();
            var modifiers = new AttributeModifier[modifiersCount];
            for (int i = 0; i < modifiers.Length; i++)
                modifiers[i] = reader.ReadType<AttributeModifier>(protocolVersion);
            return new EntityAttribute(keyName, default!, value, modifiers);
        }

        if (protocolVersion >= 766)
        {
            var keyId = reader.ReadVarInt();
            var value = reader.ReadDouble();
            int modifiersCount = reader.ReadVarInt();
            var modifiers = new AttributeModifier[modifiersCount];
            for (int i = 0; i < modifiers.Length; i++)
                modifiers[i] = reader.ReadType<AttributeModifier>(protocolVersion);
            return new EntityAttribute(default!, keyId, value, modifiers);
        }

        throw new System.NotSupportedException($"EntityAttribute has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityAttribute>(protocolVersion);
        if (protocolVersion <= 754)
        {
            writer.WriteString(KeyName);
            writer.WriteDouble(Value);
            writer.WriteVarInt(Modifiers.Length);
            foreach (var modifiersItem in Modifiers)
                writer.WriteType<AttributeModifier>(modifiersItem, protocolVersion);
            return;
        }

        if (protocolVersion >= 755 && protocolVersion <= 765)
        {
            writer.WriteString(KeyName);
            writer.WriteDouble(Value);
            writer.WriteVarInt(Modifiers.Length);
            foreach (var modifiersItem in Modifiers)
                writer.WriteType<AttributeModifier>(modifiersItem, protocolVersion);
            return;
        }

        if (protocolVersion >= 766)
        {
            writer.WriteVarInt(KeyId);
            writer.WriteDouble(Value);
            writer.WriteVarInt(Modifiers.Length);
            foreach (var modifiersItem in Modifiers)
                writer.WriteType<AttributeModifier>(modifiersItem, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"EntityAttribute has no wire layout for protocol version {protocolVersion}.");
    }
}
