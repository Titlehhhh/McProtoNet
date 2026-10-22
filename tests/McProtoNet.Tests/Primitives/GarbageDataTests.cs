using McProtoNet.Primitives;

namespace McProtoNet.Tests.Primitives;

/// <summary>
///     Wire garbage has one answer everywhere in the primitives: <see cref="InvalidDataException" />.
///     Not a buffer type of our own, not an arithmetic error, not an out-of-memory one.
/// </summary>
public class GarbageDataTests
{
    public static TheoryData<string> TruncatedReads => new()
    {
        "short", "int", "long", "float", "double", "uuid", "varint", "varlong", "exact", "span"
    };

    [Theory]
    [MemberData(nameof(TruncatedReads))]
    public void ReadPastTheEnd_ThrowsInvalidData(string what)
    {
        var data = new byte[1];
        data[0] = 0x80; // a varint that says "one more byte" and then runs out

        Assert.Throws<InvalidDataException>(() =>
        {
            var reader = new MinecraftPrimitiveReader(data);
            switch (what)
            {
                case "short": reader.ReadSignedShort(); break;
                case "int": reader.ReadSignedInt(); break;
                case "long": reader.ReadSignedLong(); break;
                case "float": reader.ReadFloat(); break;
                case "double": reader.ReadDouble(); break;
                case "uuid": reader.ReadUUID(); break;
                case "varint": reader.ReadVarInt(); break;
                case "varlong": reader.ReadVarLong(); break;
                case "exact": reader.Read(4); break;
                default: reader.Read(new byte[4]); break;
            }
        });
    }

    [Fact]
    public void VarIntLongerThanFiveBytes_ThrowsInvalidData()
    {
        var data = new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 };
        Assert.Throws<InvalidDataException>(() =>
        {
            var reader = new MinecraftPrimitiveReader(data);
            reader.ReadVarInt();
        });
    }

    [Fact]
    public void VarLongLongerThanTenBytes_ThrowsInvalidData()
    {
        var data = new byte[12];
        Array.Fill(data, (byte)0x80);
        Assert.Throws<InvalidDataException>(() =>
        {
            var reader = new MinecraftPrimitiveReader(data);
            reader.ReadVarLong();
        });
    }

    [Fact]
    public void StringLengthPastTheEnd_ThrowsInvalidData()
    {
        var data = new byte[] { 0x40, (byte)'h', (byte)'i' }; // says 64 bytes, carries 2
        Assert.Throws<InvalidDataException>(() =>
        {
            var reader = new MinecraftPrimitiveReader(data);
            reader.ReadString();
        });
    }

    [Fact]
    public void BufferLongerThanTheData_ThrowsInvalidData()
    {
        Assert.Throws<InvalidDataException>(() =>
        {
            var reader = new MinecraftPrimitiveReader(new byte[3]);
            reader.ReadBuffer(9);
        });
    }
}
