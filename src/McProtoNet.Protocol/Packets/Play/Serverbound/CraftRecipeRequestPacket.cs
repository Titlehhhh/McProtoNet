using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("CraftRecipeRequest", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class CraftRecipeRequestPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 767),
        new(768, MinecraftVersion.LatestProtocol)
    };

    public bool MakeAll { get; set; }

    public VFirst_767Fields? VFirst_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                var fields = VFirst_767 ?? throw new InvalidOperationException("CraftRecipeRequest VFirst_767 fields missing.");
                writer.WriteSignedByte(fields.WindowId);
                writer.WriteString(fields.Recipe);
                writer.WriteBoolean(MakeAll);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("CraftRecipeRequest V768_Last fields missing.");
                writer.WriteVarInt(fields.WindowId);
                writer.WriteVarInt(fields.RecipeId);
                writer.WriteBoolean(MakeAll);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.CraftRecipeRequest), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
                VFirst_767 = new VFirst_767Fields
                {
                    WindowId = reader.ReadSignedByte(),
                    Recipe = reader.ReadString()
                };
                MakeAll = reader.ReadBoolean();
                V768_Last = null;
                return;
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                V768_Last = new V768_LastFields
                {
                    WindowId = reader.ReadVarInt(),
                    RecipeId = reader.ReadVarInt()
                };
                MakeAll = reader.ReadBoolean();
                VFirst_767 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.CraftRecipeRequest), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_767Fields
    {
        public sbyte WindowId { get; set; }
        public string Recipe { get; set; }
    }

    public struct V768_LastFields
    {
        public int WindowId { get; set; }
        public int RecipeId { get; set; }
    }
}
