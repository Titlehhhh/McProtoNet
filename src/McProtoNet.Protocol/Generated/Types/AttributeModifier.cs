using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class AttributeModifier : IProtocolType<AttributeModifier>
{
    public Guid Uuid { get; }
    public string Id { get; }
    public double Amount { get; }
    public int Operation { get; }

    public AttributeModifier(Guid uuid, string id, double amount, int operation)
    {
        Uuid = uuid;
        Id = id;
        Amount = amount;
        Operation = operation;
    }

    public static AttributeModifier Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AttributeModifier>(protocolVersion);
        if (protocolVersion <= 766)
        {
            var uuid = reader.ReadUUID();
            var amount = reader.ReadDouble();
            var operation = reader.ReadSignedByte();
            return new AttributeModifier(uuid, default!, amount, operation);
        }

        if (protocolVersion >= 767)
        {
            var id = reader.ReadString();
            var amount = reader.ReadDouble();
            var operation = reader.ReadSignedByte();
            return new AttributeModifier(default!, id, amount, operation);
        }

        throw new System.NotSupportedException($"AttributeModifier has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AttributeModifier>(protocolVersion);
        if (protocolVersion <= 766)
        {
            writer.WriteUUID(Uuid);
            writer.WriteDouble(Amount);
            writer.WriteSignedByte((sbyte)Operation);
            return;
        }

        if (protocolVersion >= 767)
        {
            writer.WriteString(Id);
            writer.WriteDouble(Amount);
            writer.WriteSignedByte((sbyte)Operation);
            return;
        }

        throw new System.NotSupportedException($"AttributeModifier has no wire layout for protocol version {protocolVersion}.");
    }
}
