using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    extension(ref MinecraftPrimitiveWriter writer)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArray<T>(ReadOnlySpan<T> arr)
            => writer.WriteArray(arr, LengthFormat.VarInt);

        public void WriteArray<T>(ReadOnlySpan<T> arr, [ConstantExpected] LengthFormat lengthFormat)
        {
            WriteLength(ref writer, lengthFormat, arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                WriteTypeWithoutProtocolVersion(ref writer, arr[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArray<T>(ReadOnlySpan<T> arr, int protocolVersion)
            => writer.WriteArray(arr, LengthFormat.VarInt, protocolVersion);

        public void WriteArray<T>(ReadOnlySpan<T> arr, [ConstantExpected] LengthFormat lengthFormat, int protocolVersion)
        {
            WriteLength(ref writer, lengthFormat, arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                writer.WriteType(arr[i], protocolVersion);
            }
        }

        public void WriteBuffer(ReadOnlySpan<byte> buff, int length)
        {
            if ((uint)length > (uint)buff.Length)
            {
                ThrowHelper.ThrowBufferLengthOutOfRange(length, buff.Length);
            }
            writer.WriteBuffer(buff[..length]);
        }

        public void WriteBuffer<TLen>(ReadOnlySpan<byte> buff)
        {
            if (typeof(TLen) == typeof(VarInt))
            {
                writer.WriteVarInt(buff.Length);
                writer.WriteBuffer(buff);
            }
        }


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
                WriteRegistryEntryHolderBoxed(ref writer, typeof(T).GetGenericArguments()[0], val!, protocolVersion);
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

            if (typeof(T) == typeof(PacketCommonAddResourcePack)) { writer.WritePacketCommonAddResourcePack((PacketCommonAddResourcePack)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PacketCommonClearDialog)) { writer.WritePacketCommonClearDialog((PacketCommonClearDialog)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PacketCommonCookieRequest)) { writer.WritePacketCommonCookieRequest((PacketCommonCookieRequest)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PacketCommonCookieResponse)) { writer.WritePacketCommonCookieResponse((PacketCommonCookieResponse)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PacketCommonCustomClickAction)) { writer.WritePacketCommonCustomClickAction((PacketCommonCustomClickAction)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PacketCommonCustomReportDetails)) { writer.WritePacketCommonCustomReportDetails((PacketCommonCustomReportDetails)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PacketCommonRemoveResourcePack)) { writer.WritePacketCommonRemoveResourcePack((PacketCommonRemoveResourcePack)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PacketCommonSelectKnownPacks)) { writer.WritePacketCommonSelectKnownPacks((PacketCommonSelectKnownPacks)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PacketCommonServerLinks)) { writer.WritePacketCommonServerLinks((PacketCommonServerLinks)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PacketCommonSettings)) { writer.WritePacketCommonSettings((PacketCommonSettings)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PacketCommonStoreCookie)) { writer.WritePacketCommonStoreCookie((PacketCommonStoreCookie)(object)val!, protocolVersion); return; }
            if (typeof(T) == typeof(PacketCommonTransfer)) { writer.WritePacketCommonTransfer((PacketCommonTransfer)(object)val!, protocolVersion); return; }

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

        public void WriteVec2f(Vec2f vec, int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec2f>(protocolVersion);
            writer.WriteFloat(vec.X);
            writer.WriteFloat(vec.Y);
        }

        public void WriteVec3f(Vec3f vec, int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec3f>(protocolVersion);
            writer.WriteFloat(vec.X);
            writer.WriteFloat(vec.Y);
            writer.WriteFloat(vec.Z);
        }

        public void WriteVec3f64(Vec3f64 vec, int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec3f64>(protocolVersion);
            writer.WriteDouble(vec.X);
            writer.WriteDouble(vec.Y);
            writer.WriteDouble(vec.Z);
        }

        public void WriteVec3i(Vec3i vec, int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec3i>(protocolVersion);
            writer.WriteVarInt(vec.X);
            writer.WriteVarInt(vec.Y);
            writer.WriteVarInt(vec.Z);
        }

        public void WriteVec4f(Vec4f vec, int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec4f>(protocolVersion);
            writer.WriteFloat(vec.X);
            writer.WriteFloat(vec.Y);
            writer.WriteFloat(vec.Z);
            writer.WriteFloat(vec.W);
        }

        public void WritePosition(Position position, int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Position>(protocolVersion);
            if (protocolVersion >= 477)
            {
                var encoded = (((ulong)position.X & 0x3FFFFFF) << 38) |
                              (((ulong)position.Z & 0x3FFFFFF) << 12) |
                              ((ulong)position.Y & 0xFFF);
                writer.WriteUnsignedLong(encoded);
            }
            else
            {
                var encoded = (((ulong)position.X & 0x3FFFFFF) << 38) |
                              (((ulong)position.Y & 0xFFF) << 26) |
                              ((ulong)position.Z & 0x3FFFFFF);
                writer.WriteUnsignedLong(encoded);
            }
        }
    }

    extension(ref MinecraftPrimitiveReader reader)
    {
        public T[] ReadArray<T>(LengthFormat lengthFormat)
        {
            int length = ReadLength(ref reader, lengthFormat);
            if (length == 0)
            {
                return Array.Empty<T>();
            }

            var result = new T[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadTypeWithoutProtocolVersion<T>(ref reader);
            }

            return result;
        }

        public T[] ReadArray<T>(LengthFormat lengthFormat, int protocolVersion)
        {
            int length = ReadLength(ref reader, lengthFormat);
            return reader.ReadArray<T>(length, protocolVersion);
        }

        public T[] ReadArray<T>(int length, int protocolVersion)
        {
            if (length == 0)
            {
                return Array.Empty<T>();
            }

            var result = new T[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = reader.ReadType<T>(protocolVersion);
            }

            return result;
        }

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

            if (typeof(T) == typeof(PacketCommonAddResourcePack)) return (T)(object)reader.ReadPacketCommonAddResourcePack(protocolVersion);
            if (typeof(T) == typeof(PacketCommonClearDialog)) return (T)(object)reader.ReadPacketCommonClearDialog(protocolVersion);
            if (typeof(T) == typeof(PacketCommonCookieRequest)) return (T)(object)reader.ReadPacketCommonCookieRequest(protocolVersion);
            if (typeof(T) == typeof(PacketCommonCookieResponse)) return (T)(object)reader.ReadPacketCommonCookieResponse(protocolVersion);
            if (typeof(T) == typeof(PacketCommonCustomClickAction)) return (T)(object)reader.ReadPacketCommonCustomClickAction(protocolVersion);
            if (typeof(T) == typeof(PacketCommonCustomReportDetails)) return (T)(object)reader.ReadPacketCommonCustomReportDetails(protocolVersion);
            if (typeof(T) == typeof(PacketCommonRemoveResourcePack)) return (T)(object)reader.ReadPacketCommonRemoveResourcePack(protocolVersion);
            if (typeof(T) == typeof(PacketCommonSelectKnownPacks)) return (T)(object)reader.ReadPacketCommonSelectKnownPacks(protocolVersion);
            if (typeof(T) == typeof(PacketCommonServerLinks)) return (T)(object)reader.ReadPacketCommonServerLinks(protocolVersion);
            if (typeof(T) == typeof(PacketCommonSettings)) return (T)(object)reader.ReadPacketCommonSettings(protocolVersion);
            if (typeof(T) == typeof(PacketCommonStoreCookie)) return (T)(object)reader.ReadPacketCommonStoreCookie(protocolVersion);
            if (typeof(T) == typeof(PacketCommonTransfer)) return (T)(object)reader.ReadPacketCommonTransfer(protocolVersion);

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
        public Position ReadPosition(int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Position>(protocolVersion);
            var locEncoded = reader.ReadSignedLong();
            var x = (int)(locEncoded >> 38);
            var z = (int)((locEncoded >> 12) & 0x3FFFFFF);
            var y = (int)(locEncoded & 0xFFF);
            return new Position(x, y, z);
        }

        public Vec2f ReadVec2f(int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec2f>(protocolVersion);
            var x = reader.ReadFloat();
            var y = reader.ReadFloat();
            return new Vec2f(x, y);
        }

        public Vec3f ReadVec3f(int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec3f>(protocolVersion);
            var x = reader.ReadFloat();
            var y = reader.ReadFloat();
            var z = reader.ReadFloat();
            return new Vec3f(x, y, z);
        }

        public Vec3f64 ReadVec3f64(int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec3f64>(protocolVersion);
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            return new Vec3f64(x, y, z);
        }

        public Vec3i ReadVec3i(int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec3i>(protocolVersion);
            var x = reader.ReadVarInt();
            var y = reader.ReadVarInt();
            var z = reader.ReadVarInt();
            return new Vec3i(x, y, z);
        }

        public Vec4f ReadVec4f(int protocolVersion)
        {
            ThrowHelper.ThrowIfProtocolNotSupported<Vec4f>(protocolVersion);
            var x = reader.ReadFloat();
            var y = reader.ReadFloat();
            var z = reader.ReadFloat();
            var w = reader.ReadFloat();
            return new Vec4f(x, y, z, w);
        }
    }

    private static object ReadRegistryEntryHolderBoxed(ref MinecraftPrimitiveReader reader, Type valueType,
        int protocolVersion)
    {
        if (valueType == typeof(string)) return reader.ReadRegistryEntryHolder<string>(protocolVersion);
        if (valueType == typeof(int)) return reader.ReadRegistryEntryHolder<int>(protocolVersion);
        if (valueType == typeof(ArmorTrimMaterial)) return reader.ReadRegistryEntryHolder<ArmorTrimMaterial>(protocolVersion);
        if (valueType == typeof(ArmorTrimPattern)) return reader.ReadRegistryEntryHolder<ArmorTrimPattern>(protocolVersion);
        if (valueType == typeof(BannerPattern)) return reader.ReadRegistryEntryHolder<BannerPattern>(protocolVersion);
        if (valueType == typeof(EntityMetadataPaintingVariant))
            return reader.ReadRegistryEntryHolder<EntityMetadataPaintingVariant>(protocolVersion);
        if (valueType == typeof(EntityMetadataWolfVariant))
            return reader.ReadRegistryEntryHolder<EntityMetadataWolfVariant>(protocolVersion);
        if (valueType == typeof(InstrumentData)) return reader.ReadRegistryEntryHolder<InstrumentData>(protocolVersion);
        if (valueType == typeof(ItemSoundEvent)) return reader.ReadRegistryEntryHolder<ItemSoundEvent>(protocolVersion);
        if (valueType == typeof(JukeboxSongData)) return reader.ReadRegistryEntryHolder<JukeboxSongData>(protocolVersion);

        throw new NotSupportedException($"ReadType<RegistryEntryHolder<{valueType.Name}>> is not registered.");
    }

    private static void WriteRegistryEntryHolderBoxed(ref MinecraftPrimitiveWriter writer, Type valueType, object value,
        int protocolVersion)
    {
        if (valueType == typeof(string))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<string>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(int))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<int>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(ArmorTrimMaterial))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<ArmorTrimMaterial>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(ArmorTrimPattern))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<ArmorTrimPattern>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(BannerPattern))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<BannerPattern>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(EntityMetadataPaintingVariant))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<EntityMetadataPaintingVariant>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(EntityMetadataWolfVariant))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<EntityMetadataWolfVariant>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(InstrumentData))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<InstrumentData>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(ItemSoundEvent))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<ItemSoundEvent>)value, protocolVersion);
            return;
        }
        if (valueType == typeof(JukeboxSongData))
        {
            writer.WriteRegistryEntryHolder((RegistryEntryHolder<JukeboxSongData>)value, protocolVersion);
            return;
        }

        throw new NotSupportedException($"WriteType<RegistryEntryHolder<{valueType.Name}>> is not registered.");
    }

    private static int ReadLength(ref MinecraftPrimitiveReader reader, LengthFormat lengthFormat)
    {
        return lengthFormat switch
        {
            LengthFormat.VarInt => reader.ReadVarInt(),
            LengthFormat.Byte => reader.ReadUnsignedByte(),
            LengthFormat.Short => reader.ReadSignedShort(),
            LengthFormat.Int => reader.ReadSignedInt(),
            _ => throw new ArgumentOutOfRangeException(nameof(lengthFormat), lengthFormat, null)
        };
    }

    private static void WriteLength(ref MinecraftPrimitiveWriter writer, LengthFormat lengthFormat, int length)
    {
        switch (lengthFormat)
        {
            case LengthFormat.VarInt:
                writer.WriteVarInt(length);
                return;
            case LengthFormat.Byte:
                writer.WriteUnsignedByte((byte)length);
                return;
            case LengthFormat.Short:
                writer.WriteSignedShort((short)length);
                return;
            case LengthFormat.Int:
                writer.WriteSignedInt(length);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(lengthFormat), lengthFormat, null);
        }
    }

    private static T ReadTypeWithoutProtocolVersion<T>(ref MinecraftPrimitiveReader reader)
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

        throw new NotSupportedException(
            $"ReadArray<{typeof(T).Name}> without protocolVersion is not supported. Use ReadArray<T>(..., int).");
    }

    private static void WriteTypeWithoutProtocolVersion<T>(ref MinecraftPrimitiveWriter writer, T value)
    {
        if (typeof(T) == typeof(bool)) { writer.WriteBoolean((bool)(object)value!); return; }
        if (typeof(T) == typeof(byte)) { writer.WriteUnsignedByte((byte)(object)value!); return; }
        if (typeof(T) == typeof(sbyte)) { writer.WriteSignedByte((sbyte)(object)value!); return; }
        if (typeof(T) == typeof(short)) { writer.WriteSignedShort((short)(object)value!); return; }
        if (typeof(T) == typeof(ushort)) { writer.WriteUnsignedShort((ushort)(object)value!); return; }
        if (typeof(T) == typeof(int)) { writer.WriteSignedInt((int)(object)value!); return; }
        if (typeof(T) == typeof(uint)) { writer.WriteUnsignedInt((uint)(object)value!); return; }
        if (typeof(T) == typeof(long)) { writer.WriteSignedLong((long)(object)value!); return; }
        if (typeof(T) == typeof(ulong)) { writer.WriteUnsignedLong((ulong)(object)value!); return; }
        if (typeof(T) == typeof(float)) { writer.WriteFloat((float)(object)value!); return; }
        if (typeof(T) == typeof(double)) { writer.WriteDouble((double)(object)value!); return; }
        if (typeof(T) == typeof(string)) { writer.WriteString((string)(object)value!); return; }
        if (typeof(T) == typeof(Guid)) { writer.WriteUUID((Guid)(object)value!); return; }

        throw new NotSupportedException(
            $"WriteArray<{typeof(T).Name}> without protocolVersion is not supported. Use WriteArray<T>(..., int).");
    }
}
