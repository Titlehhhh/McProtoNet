using System.Buffers;
using System.IO;
using System.Threading;
using McProtoNet.Serialization;

namespace McProtoNet.Benchmark.Pipelines;

public class TestPacket
{
    private int _entityId;

    public string Name { get; set; }

    public int EntityId
    {
        get => _entityId;
        set => _entityId = value;
    }

    private short _dx;

    public short DX
    {
        get => _dx;
        set => _dx = value;
    }

    private short _dy;

    public short DY
    {
        get => _dy;
        set => _dy = value;
    }

    private short _dz;

    public short DZ
    {
        get => _dz;
        set => _dz = value;
    }

    private sbyte _yaw;

    public sbyte Yaw
    {
        get => _yaw;
        set => _yaw = value;
    }

    private sbyte _pitch;

    public sbyte Pitch
    {
        get => _pitch;
        set => _pitch = value;
    }

    private bool _onGround;

    public bool OnGround
    {
        get => _onGround;
        set => _onGround = value;
    }

    //public byte[] SpecialData { get; set; }

    public void Serialize(ref MinecraftPrimitiveWriter writer)
    {
        //writer.WriteString(Name);
        writer.WriteVarInt(EntityId);
        writer.WriteSignedShort(DX);
        writer.WriteSignedShort(DY);
        writer.WriteSignedShort(DZ);
        writer.WriteSignedByte(Yaw);
        writer.WriteSignedByte(Pitch);
        writer.WriteBoolean(OnGround);

        // writer.WriteVarInt(SpecialData.Length);
        // writer.WriteBuffer(SpecialData);
    }

    public void Deserialize(ref MinecraftPrimitiveReader reader)
    {
        //Name = reader.ReadString();
        EntityId = reader.ReadVarInt();
        DX = reader.ReadSignedShort();
        DY = reader.ReadSignedShort();
        DZ = reader.ReadSignedShort();
        Yaw = reader.ReadSignedByte();
        Pitch = reader.ReadSignedByte();
        OnGround = reader.ReadBoolean();

        //SpecialData = reader.ReadBuffer(reader.ReadVarInt());
    }

    public void Deserialize(ref SequenceReader<byte> reader)
    {
        //reader.TryReadString(out var name);
        //Name = name;
        bool success = true;
        success &= reader.TryReadVarInt(out _entityId, out _);
        success &= reader.TryReadBigEndian(out _dx);
        success &= reader.TryReadBigEndian(out _dy);
        success &= reader.TryReadBigEndian(out _dz);
        success &= reader.TryRead(out var yaw);
        Yaw = (sbyte)yaw;

        success &= reader.TryRead(out var pitch);
        Pitch = (sbyte)pitch;

        success &= reader.TryRead(out var onGround);
        OnGround = onGround == 1;

        if (!success)
            throw new EndOfStreamException("Где то Try не сработал");

        //reader.TryReadVarInt(out var specialDataLength, out _);

        // var specialData = new byte[specialDataLength];
        // reader.TryCopyTo(specialData);
        // SpecialData = specialData;
    }
}