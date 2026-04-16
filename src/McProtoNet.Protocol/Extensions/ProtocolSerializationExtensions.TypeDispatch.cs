using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    extension(MinecraftPrimitiveWriter writer)
    {
        public void WriteType<T>(T val)
        {
            throw new NotSupportedException("WriteType<T> requires protocolVersion. Use WriteType<T>(T, int).");
        }

        public void WriteType<T>(T val, int protocolVersion)
        {
            if (typeof(T) == typeof(bool)) { writer.WriteBoolean((bool)(object)val!); return; }
            if (typeof(T) == typeof(byte)) { writer.WriteUnsignedByte((byte)(object)val!); return; }
            if (typeof(T) == typeof(sbyte)) { writer.WriteSignedByte((sbyte)(object)val!); return; }
            if (typeof(T) == typeof(short)) { writer.WriteSignedShort((short)(object)val!); return; }
            if (typeof(T) == typeof(ushort)) { writer.WriteUnsignedShort((ushort)(object)val!); return; }
            if (typeof(T) == typeof(int)) { writer.WriteSignedInt((int)(object)val!); return; }
            if (typeof(T) == typeof(uint)) { writer.WriteUnsignedInt((uint)(object)val!); return; }
            if (typeof(T) == typeof(long)) { writer.WriteSignedLong((long)(object)val!); return; }
            if (typeof(T) == typeof(ulong)) { writer.WriteUnsignedLong((ulong)(object)val!); return; }
            if (typeof(T) == typeof(float)) { writer.WriteFloat((float)(object)val!); return; }
            if (typeof(T) == typeof(double)) { writer.WriteDouble((double)(object)val!); return; }
            if (typeof(T) == typeof(string)) { writer.WriteString((string)(object)val!); return; }
            if (typeof(T) == typeof(Guid)) { writer.WriteUUID((Guid)(object)val!); return; }

            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(RegistryEntryHolder<>))
            {
                WriteRegistryEntryHolderBoxed(writer, typeof(T).GetGenericArguments()[0], val!, protocolVersion);
                return;
            }

            if (typeof(T) == typeof(Position)) { writer.WritePosition((Position)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(Vec2f)) { writer.WriteVec2f((Vec2f)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(Vec3f)) { writer.WriteVec3f((Vec3f)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(Vec3f64)) { writer.WriteVec3f64((Vec3f64)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(Vec3i)) { writer.WriteVec3i((Vec3i)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(Vec4f)) { writer.WriteVec4f((Vec4f)(object)val!, protocolVersion); return; }

            if (typeof(T) == typeof(Slot)) { writer.WriteSlot((Slot)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(HashedSlot)) { writer.WriteHashedSlot((HashedSlot)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(UntrustedSlot)) { writer.WriteUntrustedSlot((UntrustedSlot)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(SlotComponent)) { writer.WriteSlotComponent((SlotComponent)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(UntrustedSlotComponent)) { writer.WriteUntrustedSlotComponent((UntrustedSlotComponent)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(SlotComponentType)) { writer.WriteSlotComponentType((SlotComponentType)(object)val!, protocolVersion); return; }

            if (typeof(T) == typeof(BannerPattern)) { writer.WriteBannerPattern((BannerPattern)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(DataComponentMatchers)) { writer.WriteDataComponentMatchers((DataComponentMatchers)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ExactComponentMatcher)) { writer.WriteExactComponentMatcher((ExactComponentMatcher)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(GameProfile)) { writer.WriteGameProfile((GameProfile)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ItemBlockProperty)) { writer.WriteItemBlockProperty((ItemBlockProperty)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ItemEffectDetail)) { writer.WriteItemEffectDetail((ItemEffectDetail)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ItemSoundEvent)) { writer.WriteItemSoundEvent((ItemSoundEvent)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(MinecraftSimpleRecipeFormat)) { writer.WriteMinecraftSimpleRecipeFormat((MinecraftSimpleRecipeFormat)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PackedChunkPos)) { writer.WritePackedChunkPos((PackedChunkPos)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ChatSession)) { writer.WriteChatSession((ChatSession)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(Particle)) { writer.WriteParticle((Particle)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PreviousMessages)) { writer.WritePreviousMessages((PreviousMessages)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ServerLinkType)) { writer.WriteServerLinkType((ServerLinkType)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(SoundSource)) { writer.WriteSoundSource((SoundSource)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(Tags)) { writer.WriteTags((Tags)(object)val!, protocolVersion); return; }

            if (typeof(T) == typeof(ArmorTrimMaterial)) { writer.WriteArmorTrimMaterial((ArmorTrimMaterial)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ArmorTrimPattern)) { writer.WriteArmorTrimPattern((ArmorTrimPattern)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(BannerPatternLayer)) { writer.WriteBannerPatternLayer((BannerPatternLayer)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(EntityMetadataPaintingVariant)) { writer.WriteEntityMetadataPaintingVariant((EntityMetadataPaintingVariant)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(EntityMetadataWolfVariant)) { writer.WriteEntityMetadataWolfVariant((EntityMetadataWolfVariant)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(IDSet)) { writer.WriteIDSet((IDSet)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(InstrumentData)) { writer.WriteInstrumentData((InstrumentData)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ItemBlockPredicate)) { writer.WriteItemBlockPredicate((ItemBlockPredicate)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ItemBookPage)) { writer.WriteItemBookPage((ItemBookPage)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ItemConsumeEffect)) { writer.WriteItemConsumeEffect((ItemConsumeEffect)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ItemFireworkExplosion)) { writer.WriteItemFireworkExplosion((ItemFireworkExplosion)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ItemPotionEffect)) { writer.WriteItemPotionEffect((ItemPotionEffect)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ItemSoundHolder)) { writer.WriteItemSoundHolder((ItemSoundHolder)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ItemWrittenBookPage)) { writer.WriteItemWrittenBookPage((ItemWrittenBookPage)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(JukeboxSongData)) { writer.WriteJukeboxSongData((JukeboxSongData)(object)val!, protocolVersion); return; }
            
            if (typeof(T) == typeof(ChatType)) { writer.WriteChatType((ChatType)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ChatTypeParameterType)) { writer.WriteChatTypeParameterType((ChatTypeParameterType)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ChatTypes)) { writer.WriteChatTypes((ChatTypes)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(ChatTypesHolder)) { writer.WriteChatTypesHolder((ChatTypesHolder)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PositionUpdateRelatives)) { writer.WritePositionUpdateRelatives((PositionUpdateRelatives)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(RecipeBookSetting)) { writer.WriteRecipeBookSetting((RecipeBookSetting)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(RecipeDisplay)) { writer.WriteRecipeDisplay((RecipeDisplay)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(SlotDisplay)) { writer.WriteSlotDisplay((SlotDisplay)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(MovementFlags)) { writer.WriteMovementFlags((MovementFlags)(object)val!, protocolVersion); return; }

            throw new NotSupportedException($"WriteType<{typeof(T).Name}> is not registered.");
        }
    }

    extension(ref MinecraftPrimitiveReader reader)
    {
        public T ReadType<T>(int protocolVersion)
        {
            if (typeof(T) == typeof(bool)) return (T)(object)reader.ReadBoolean();
            if (typeof(T) == typeof(byte)) return (T)(object)reader.ReadUnsignedByte();
            if (typeof(T) == typeof(sbyte)) return (T)(object)reader.ReadSignedByte();
            if (typeof(T) == typeof(short)) return (T)(object)reader.ReadSignedShort();
            if (typeof(T) == typeof(ushort)) return (T)(object)reader.ReadUnsignedShort();
            if (typeof(T) == typeof(int)) return (T)(object)reader.ReadSignedInt();
            if (typeof(T) == typeof(uint)) return (T)(object)reader.ReadUnsignedInt();
            if (typeof(T) == typeof(long)) return (T)(object)reader.ReadSignedLong();
            if (typeof(T) == typeof(ulong)) return (T)(object)reader.ReadUnsignedLong();
            if (typeof(T) == typeof(float)) return (T)(object)reader.ReadFloat();
            if (typeof(T) == typeof(double)) return (T)(object)reader.ReadDouble();
            if (typeof(T) == typeof(string)) return (T)(object)reader.ReadString();
            if (typeof(T) == typeof(Guid)) return (T)(object)reader.ReadUUID();

            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(RegistryEntryHolder<>))
            {
                object holder = ReadRegistryEntryHolderBoxed(ref reader, typeof(T).GetGenericArguments()[0], protocolVersion);
                return (T)holder;
            }

            if (typeof(T) == typeof(Position)) return (T)(object)reader.ReadPosition(protocolVersion);
            if (typeof(T) == typeof(Vec2f)) return (T)(object)reader.ReadVec2f(protocolVersion);
            if (typeof(T) == typeof(Vec3f)) return (T)(object)reader.ReadVec3f(protocolVersion);
            if (typeof(T) == typeof(Vec3f64)) return (T)(object)reader.ReadVec3f64(protocolVersion);
            if (typeof(T) == typeof(Vec3i)) return (T)(object)reader.ReadVec3i(protocolVersion);
            if (typeof(T) == typeof(Vec4f)) return (T)(object)reader.ReadVec4f(protocolVersion);

            if (typeof(T) == typeof(Slot)) return (T)(object)reader.ReadSlot(protocolVersion);
            if (typeof(T) == typeof(HashedSlot)) return (T)(object)reader.ReadHashedSlot(protocolVersion);
            if (typeof(T) == typeof(UntrustedSlot)) return (T)(object)reader.ReadUntrustedSlot(protocolVersion);
            if (typeof(T) == typeof(SlotComponent)) return (T)(object)reader.ReadSlotComponent(protocolVersion);
            if (typeof(T) == typeof(UntrustedSlotComponent)) return (T)(object)reader.ReadUntrustedSlotComponent(protocolVersion);
            if (typeof(T) == typeof(SlotComponentType)) return (T)(object)reader.ReadSlotComponentType(protocolVersion);

            if (typeof(T) == typeof(BannerPattern)) return (T)(object)reader.ReadBannerPattern(protocolVersion);
            if (typeof(T) == typeof(DataComponentMatchers)) return (T)(object)reader.ReadDataComponentMatchers(protocolVersion);
            if (typeof(T) == typeof(ExactComponentMatcher)) return (T)(object)reader.ReadExactComponentMatcher(protocolVersion);
            if (typeof(T) == typeof(GameProfile)) return (T)(object)reader.ReadGameProfile(protocolVersion);
            if (typeof(T) == typeof(ItemBlockProperty)) return (T)(object)reader.ReadItemBlockProperty(protocolVersion);
            if (typeof(T) == typeof(ItemEffectDetail)) return (T)(object)reader.ReadItemEffectDetail(protocolVersion);
            if (typeof(T) == typeof(ItemSoundEvent)) return (T)(object)reader.ReadItemSoundEvent(protocolVersion);
            if (typeof(T) == typeof(MinecraftSimpleRecipeFormat)) return (T)(object)reader.ReadMinecraftSimpleRecipeFormat(protocolVersion);
            if (typeof(T) == typeof(PackedChunkPos)) return (T)(object)reader.ReadPackedChunkPos(protocolVersion);
            if (typeof(T) == typeof(ChatSession)) return (T)(object)reader.ReadChatSession(protocolVersion);
            if (typeof(T) == typeof(Particle)) return (T)(object)reader.ReadParticle(protocolVersion);
            if (typeof(T) == typeof(PreviousMessages)) return (T)(object)reader.ReadPreviousMessages(protocolVersion);
            if (typeof(T) == typeof(ServerLinkType)) return (T)(object)reader.ReadServerLinkType(protocolVersion);
            if (typeof(T) == typeof(SoundSource)) return (T)(object)reader.ReadSoundSource(protocolVersion);
            if (typeof(T) == typeof(Tags)) return (T)(object)reader.ReadTags(protocolVersion);

            if (typeof(T) == typeof(ArmorTrimMaterial)) return (T)(object)reader.ReadArmorTrimMaterial(protocolVersion);
            if (typeof(T) == typeof(ArmorTrimPattern)) return (T)(object)reader.ReadArmorTrimPattern(protocolVersion);
            if (typeof(T) == typeof(BannerPatternLayer)) return (T)(object)reader.ReadBannerPatternLayer(protocolVersion);
            if (typeof(T) == typeof(EntityMetadataPaintingVariant)) return (T)(object)reader.ReadEntityMetadataPaintingVariant(protocolVersion);
            if (typeof(T) == typeof(EntityMetadataWolfVariant)) return (T)(object)reader.ReadEntityMetadataWolfVariant(protocolVersion);
            if (typeof(T) == typeof(IDSet)) return (T)(object)reader.ReadIDSet(protocolVersion);
            if (typeof(T) == typeof(InstrumentData)) return (T)(object)reader.ReadInstrumentData(protocolVersion);
            if (typeof(T) == typeof(ItemBlockPredicate)) return (T)(object)reader.ReadItemBlockPredicate(protocolVersion);
            if (typeof(T) == typeof(ItemBookPage)) return (T)(object)reader.ReadItemBookPage(protocolVersion);
            if (typeof(T) == typeof(ItemConsumeEffect)) return (T)(object)reader.ReadItemConsumeEffect(protocolVersion);
            if (typeof(T) == typeof(ItemFireworkExplosion)) return (T)(object)reader.ReadItemFireworkExplosion(protocolVersion);
            if (typeof(T) == typeof(ItemPotionEffect)) return (T)(object)reader.ReadItemPotionEffect(protocolVersion);
            if (typeof(T) == typeof(ItemSoundHolder)) return (T)(object)reader.ReadItemSoundHolder(protocolVersion);
            if (typeof(T) == typeof(ItemWrittenBookPage)) return (T)(object)reader.ReadItemWrittenBookPage(protocolVersion);
            if (typeof(T) == typeof(JukeboxSongData)) return (T)(object)reader.ReadJukeboxSongData(protocolVersion);
            
            if (typeof(T) == typeof(ChatType)) return (T)(object)reader.ReadChatType(protocolVersion);
            if (typeof(T) == typeof(ChatTypeParameterType)) return (T)(object)reader.ReadChatTypeParameterType(protocolVersion);
            if (typeof(T) == typeof(ChatTypes)) return (T)(object)reader.ReadChatTypes(protocolVersion);
            if (typeof(T) == typeof(ChatTypesHolder)) return (T)(object)reader.ReadChatTypesHolder(protocolVersion);
            if (typeof(T) == typeof(PositionUpdateRelatives)) return (T)(object)reader.ReadPositionUpdateRelatives(protocolVersion);
            if (typeof(T) == typeof(RecipeBookSetting)) return (T)(object)reader.ReadRecipeBookSetting(protocolVersion);
            if (typeof(T) == typeof(RecipeDisplay)) return (T)(object)reader.ReadRecipeDisplay(protocolVersion);
            if (typeof(T) == typeof(SlotDisplay)) return (T)(object)reader.ReadSlotDisplay(protocolVersion);
            if (typeof(T) == typeof(MovementFlags)) return (T)(object)reader.ReadMovementFlags(protocolVersion);

            throw new NotSupportedException($"ReadType<{typeof(T).Name}> is not registered.");
        }
    }
}
