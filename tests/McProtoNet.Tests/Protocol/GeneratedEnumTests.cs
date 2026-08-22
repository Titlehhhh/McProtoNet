using System.Buffers;
using McProtoNet.Protocol;
using McProtoNet.Primitives;
using PlayCb = McProtoNet.Protocol.Packets.Play.Clientbound;
using PlaySb = McProtoNet.Protocol.Packets.Play.Serverbound;

namespace McProtoNet.Tests.Protocol;

/// <summary>
/// Round-trip tests for the generated protocol enums (Generated/Enums). A generated enum is a
/// record struct over one int, not a C# enum: an id the version's table does not name must come
/// back with its raw value instead of throwing.
/// </summary>
public class GeneratedEnumTests
{
    private static T RoundTrip<T>(T value, int pv) where T : IProtocolType<T>
    {
        var writer = new MinecraftPrimitiveWriter();
        value.Write(writer, pv);
        using var mem = writer.GetWrittenMemory();
        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(mem.Memory));
        return T.Read(ref reader, pv);
    }

    [Theory]
    [InlineData(766)]
    [InlineData(772)]
    public void ChatTypeParameterType_NamedValues_RoundTrip(int pv)
    {
        Assert.Equal(ChatTypeParameterType.Content, RoundTrip(ChatTypeParameterType.Content, pv));
        Assert.Equal(ChatTypeParameterType.Sender, RoundTrip(ChatTypeParameterType.Sender, pv));
        Assert.Equal(ChatTypeParameterType.Target, RoundTrip(ChatTypeParameterType.Target, pv));
    }

    [Theory]
    [InlineData(770)]
    [InlineData(772)]
    public void SoundSource_NamedValues_RoundTrip(int pv)
    {
        Assert.Equal(SoundSource.Master, RoundTrip(SoundSource.Master, pv));
        Assert.Equal(SoundSource.Voice, RoundTrip(SoundSource.Voice, pv));
    }

    [Fact]
    public void SoundSource_UnknownId_KeepsRawValue()
    {
        var unknown = new SoundSource(99);
        Assert.Equal(99, RoundTrip(unknown, 772).Value);
        Assert.Equal("unknown(99)", unknown.ToString());
    }

    [Fact]
    public void SoundSource_Ui_IsNamedFrom771Only()
    {
        Assert.Equal("ui", SoundSource.Ui.ToString(772));
        Assert.Equal("unknown(10)", SoundSource.Ui.ToString(770));
        Assert.Equal("voice", SoundSource.Voice.ToString(770));
        Assert.Equal("ui", SoundSource.Ui.ToString());
    }

    [Theory]
    [InlineData(770)]
    [InlineData(772)]
    public void Difficulty_ChangesBacking_ButRoundTripsBothSides(int pv)
    {
        Assert.Equal(Difficulty.Peaceful, RoundTrip(Difficulty.Peaceful, pv));
        Assert.Equal(Difficulty.Hard, RoundTrip(Difficulty.Hard, pv));
        Assert.Equal(7, RoundTrip(new Difficulty(7), pv).Value);
    }

    [Fact]
    public void Difficulty_Conversions_AreExplicitAndLossless()
    {
        Assert.Equal(2, (int)Difficulty.Normal);
        Assert.Equal(Difficulty.Normal, (Difficulty)2);
        Assert.Equal("normal", Difficulty.Normal.ToString());
        Assert.Equal("unknown(9)", new Difficulty(9).ToString());
    }

    [Theory]
    [InlineData(766)]
    [InlineData(772)]
    public void Gamemode_NamedValues_RoundTrip(int pv)
    {
        Assert.Equal(Gamemode.Survival, RoundTrip(Gamemode.Survival, pv));
        Assert.Equal(Gamemode.Spectator, RoundTrip(Gamemode.Spectator, pv));
        Assert.Equal("spectator", Gamemode.Spectator.ToString());
    }

    [Theory]
    [InlineData(768)]
    [InlineData(772)]
    public void ParticleStatus_NamedValues_RoundTrip(int pv)
    {
        Assert.Equal(ParticleStatus.All, RoundTrip(ParticleStatus.All, pv));
        Assert.Equal(ParticleStatus.Minimal, RoundTrip(ParticleStatus.Minimal, pv));
    }

    [Fact]
    public void SoundSource_BeforeSupportSpan_Throws()
    {
        Assert.Throws<ProtocolNotSupportException>(() => RoundTrip(SoundSource.Master, 760));
    }

    // The enums travel inside real packets, through both backings the protocol uses for them.

    [Theory]
    [InlineData(770)]
    [InlineData(772)]
    public void DifficultyPacket_CarriesTheEnum(int pv)
    {
        var p = new PlayCb.DifficultyPacket(Difficulty.Hard, true);
        var back = RoundTrip(p, pv);

        Assert.Equal(Difficulty.Hard, back.Difficulty);
        Assert.True(back.DifficultyLocked);
    }

    [Theory]
    [InlineData(770)]
    [InlineData(772)]
    public void SetDifficultyPacket_CarriesTheEnum(int pv)
    {
        Assert.Equal(Difficulty.Easy, RoundTrip(new PlaySb.SetDifficultyPacket(Difficulty.Easy), pv).NewDifficulty);
    }

    [Fact]
    public void ChangeGamemodePacket_CarriesTheEnum()
    {
        Assert.Equal(Gamemode.Creative, RoundTrip(new PlaySb.ChangeGamemodePacket(Gamemode.Creative), 772).Mode);
    }
}
