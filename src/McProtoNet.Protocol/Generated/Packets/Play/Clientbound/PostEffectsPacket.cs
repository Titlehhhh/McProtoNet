using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(777, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.post_effects", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("PostEffects", "string[]")]
public sealed partial record PostEffectsPacket(string[] PostEffects) : IPacket<PostEffectsPacket>, IPacket
{
    public static PostEffectsPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PostEffectsPacket>(protocolVersion);
        int postEffectsCount = reader.ReadVarInt();
        var postEffects = new string[postEffectsCount];
        for (int i = 0; i < postEffects.Length; i++)
            postEffects[i] = reader.ReadString();
        return new PostEffectsPacket(postEffects);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PostEffectsPacket>(protocolVersion);
        writer.WriteVarInt(PostEffects.Length);
        foreach (var postEffectsItem in PostEffects)
            writer.WriteString(postEffectsItem);
    }

    public static PacketIdentity Identity => new("play.toClient.post_effects", "PostEffects", PacketPhase.Play, PacketDirection.Clientbound, 78);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
