using System.Buffers;
using System.Text;
using System.Text.Json;
using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Primitives;
using ConfigCb = McProtoNet.Protocol.Packets.Configuration.Clientbound;
using PlayCb = McProtoNet.Protocol.Packets.Play.Clientbound;
using PlaySb = McProtoNet.Protocol.Packets.Play.Serverbound;

namespace McProtoNet.Tests.Protocol;

/// <summary>
/// The JSON model view of generated packets: what a decoded packet looks like once it is written
/// with <see cref="IPacket.WriteJson"/>. Version layers must flatten, union cases must carry their
/// plain names, and absent optionals must not appear.
/// </summary>
public class PacketJsonTests
{
    private static string Json(IPacket packet)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            packet.WriteJson(writer);
        }

        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    private static T Decoded<T>(T value, int pv) where T : IProtocolType<T>
    {
        var writer = new MinecraftPrimitiveWriter();
        value.Write(writer, pv);
        using var mem = writer.GetWrittenMemory();
        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(mem.Memory));
        return T.Read(ref reader, pv);
    }

    [Fact]
    public void Disconnect_765_FlattensTheLayer()
    {
        var reason = new NbtCompound("") { new NbtString("text", "Bye") };
        var p = new ConfigCb.DisconnectPacket(V765_Last: new ConfigCb.DisconnectPacket.V765_LastLayer(reason));

        Assert.Equal("""{"Reason":{"text":"Bye"}}""", Json(Decoded(p, 765)));
    }

    [Fact]
    public void Disconnect_764_FlattensTheLayer()
    {
        var p = new ConfigCb.DisconnectPacket(V764: new ConfigCb.DisconnectPacket.V764Layer("{\"text\":\"Bye\"}"));

        Assert.Equal("""{"ReasonJson":"{\u0022text\u0022:\u0022Bye\u0022}"}""", Json(Decoded(p, 764)));
    }

    [Fact]
    public void Teams_776_NamesTheCaseWithoutTheVersion()
    {
        var name = new NbtCompound("") { new NbtString("text", "Red") };
        var prefix = new NbtCompound("") { new NbtString("text", "[R] ") };
        var suffix = new NbtCompound("") { new NbtString("text", "") };
        var action = new TeamAction.CreatedV776_Last(
            name, prefix, suffix, 0, 1, null, new TeamFlags(true, false), new[] { "Steve", "Alex" });
        var p = new PlayCb.TeamsPacket("red", action);

        Assert.Equal(
            """{"TeamName":"red","Action":{"$case":"Created","Name":{"text":"Red"},"Prefix":{"text":"[R] "},"Suffix":{"text":""},"NameTagVisibility":0,"CollisionRule":1,"Flags":{"FriendlyFire":true,"SeeFriendlyInvisible":false},"Players":["Steve","Alex"]}}""",
            Json(Decoded(p, 776)));
    }

    [Fact]
    public void Teams_764_SameCaseNameOnTheOldShape()
    {
        var action = new TeamAction.CreatedVUntil764("Red", 1, "always", "always", 0, "[R] ", "", new[] { "Steve" });
        var p = new PlayCb.TeamsPacket("red", action);

        var json = Json(Decoded(p, 764));

        Assert.Contains("\"$case\":\"Created\"", json);
        Assert.DoesNotContain("VUntil764", json);
    }

    [Fact]
    public void Teams_EmptyCase_IsJustTheName()
    {
        var p = new PlayCb.TeamsPacket("red", new TeamAction.Removed());

        Assert.Equal("""{"TeamName":"red","Action":{"$case":"Removed"}}""", Json(Decoded(p, 776)));
    }

    [Fact]
    public void Position_NaN_GoesOutAsAString()
    {
        var p = new PlaySb.PositionPacket(double.NaN, 64.5, double.PositiveInfinity,
            VUntil767: new PlaySb.PositionPacket.VUntil767Layer(true));

        Assert.Equal("""{"X":"NaN","Y":64.5,"Z":"Infinity","OnGround":true}""", Json(p));
    }

    [Fact]
    public void Disconnect_TwoLayersSet_WritesOnlyTheFirst()
    {
        var reason = new NbtCompound("") { new NbtString("text", "Bye") };
        var p = new ConfigCb.DisconnectPacket(
            V764: new ConfigCb.DisconnectPacket.V764Layer("old"),
            V765_Last: new ConfigCb.DisconnectPacket.V765_LastLayer(reason));

        Assert.Equal("""{"ReasonJson":"old"}""", Json(p));
    }

    [Fact]
    public void Unknown_WritesIdPhaseDirection()
    {
        var p = new UnknownPacket(0x2A, PacketPhase.Play, PacketDirection.Clientbound);

        Assert.Equal("""{"Id":42,"Phase":"Play","Direction":"Clientbound"}""", Json(p));
    }
}
