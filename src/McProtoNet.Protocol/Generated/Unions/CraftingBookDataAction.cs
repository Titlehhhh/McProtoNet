using Dunet;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, 736)]
[Union]
public partial record CraftingBookDataAction
{
    partial record DisplayedRecipe(string RecipeId);
    partial record BookSettings(bool CraftingBookOpen, bool CraftingFilter, bool SmeltingBookOpen, bool SmeltingFilter, bool BlastingBookOpen, bool BlastingFilter, bool SmokingBookOpen, bool SmokingFilter);
    public static CraftingBookDataAction Read(ref MinecraftPrimitiveReader reader, int protocolVersion, int discriminator)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CraftingBookDataAction>(protocolVersion);
        switch (discriminator)
        {
            case 0:
            {
                var recipeId = reader.ReadString();
                return new DisplayedRecipe(recipeId);
            }

            case 1:
            {
                var craftingBookOpen = reader.ReadBoolean();
                var craftingFilter = reader.ReadBoolean();
                var smeltingBookOpen = reader.ReadBoolean();
                var smeltingFilter = reader.ReadBoolean();
                var blastingBookOpen = reader.ReadBoolean();
                var blastingFilter = reader.ReadBoolean();
                var smokingBookOpen = reader.ReadBoolean();
                var smokingFilter = reader.ReadBoolean();
                return new BookSettings(craftingBookOpen, craftingFilter, smeltingBookOpen, smeltingFilter, blastingBookOpen, blastingFilter, smokingBookOpen, smokingFilter);
            }
        }

        throw new System.NotSupportedException($"CraftingBookDataAction has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CraftingBookDataAction>(protocolVersion);
        switch (this)
        {
            case DisplayedRecipe arm:
            {
                string RecipeId = arm.RecipeId;
                writer.WriteString(RecipeId);
                return;
            }

            case BookSettings arm:
            {
                bool CraftingBookOpen = arm.CraftingBookOpen;
                bool CraftingFilter = arm.CraftingFilter;
                bool SmeltingBookOpen = arm.SmeltingBookOpen;
                bool SmeltingFilter = arm.SmeltingFilter;
                bool BlastingBookOpen = arm.BlastingBookOpen;
                bool BlastingFilter = arm.BlastingFilter;
                bool SmokingBookOpen = arm.SmokingBookOpen;
                bool SmokingFilter = arm.SmokingFilter;
                writer.WriteBoolean(CraftingBookOpen);
                writer.WriteBoolean(CraftingFilter);
                writer.WriteBoolean(SmeltingBookOpen);
                writer.WriteBoolean(SmeltingFilter);
                writer.WriteBoolean(BlastingBookOpen);
                writer.WriteBoolean(BlastingFilter);
                writer.WriteBoolean(SmokingBookOpen);
                writer.WriteBoolean(SmokingFilter);
                return;
            }
        }

        throw new System.NotSupportedException($"CraftingBookDataAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }

    public int Discriminator(int protocolVersion)
    {
        switch (this)
        {
            case DisplayedRecipe _:
                return 0;
            case BookSettings _:
                return 1;
        }

        throw new System.NotSupportedException($"CraftingBookDataAction case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        switch (this)
        {
            case DisplayedRecipe arm:
            {
                writer.WriteString("$case", "DisplayedRecipe");
                writer.WritePropertyName("RecipeId");
                writer.WriteStringValue(arm.RecipeId);
                break;
            }

            case BookSettings arm:
            {
                writer.WriteString("$case", "BookSettings");
                writer.WritePropertyName("CraftingBookOpen");
                writer.WriteBooleanValue(arm.CraftingBookOpen);
                writer.WritePropertyName("CraftingFilter");
                writer.WriteBooleanValue(arm.CraftingFilter);
                writer.WritePropertyName("SmeltingBookOpen");
                writer.WriteBooleanValue(arm.SmeltingBookOpen);
                writer.WritePropertyName("SmeltingFilter");
                writer.WriteBooleanValue(arm.SmeltingFilter);
                writer.WritePropertyName("BlastingBookOpen");
                writer.WriteBooleanValue(arm.BlastingBookOpen);
                writer.WritePropertyName("BlastingFilter");
                writer.WriteBooleanValue(arm.BlastingFilter);
                writer.WritePropertyName("SmokingBookOpen");
                writer.WriteBooleanValue(arm.SmokingBookOpen);
                writer.WritePropertyName("SmokingFilter");
                writer.WriteBooleanValue(arm.SmokingFilter);
                break;
            }

            default:
                throw new System.NotSupportedException($"CraftingBookDataAction case {GetType().Name} has no JSON view.");
        }

        writer.WriteEndObject();
    }
}
