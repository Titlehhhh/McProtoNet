using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, 769)]
public sealed partial class EntityMetadataWolfVariant
{
    public string WildTexture { get; }
    public string TameTexture { get; }
    public string AngryTexture { get; }
    public IDSet Biome { get; }

    public EntityMetadataWolfVariant(string wildTexture, string tameTexture, string angryTexture, IDSet biome)
    {
        WildTexture = wildTexture;
        TameTexture = tameTexture;
        AngryTexture = angryTexture;
        Biome = biome;
    }
}
