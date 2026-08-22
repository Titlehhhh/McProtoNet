using System;
using System.Diagnostics.CodeAnalysis;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

public readonly record struct RegistryOrInline<T> : IProtocolType<RegistryOrInline<T>>
    where T : IProtocolType<T>
{
    private readonly int _tag;
    private readonly T? _value;

    private RegistryOrInline(int tag, T? value)
    {
        _tag = tag;
        _value = value;
    }

    public static RegistryOrInline<T> FromRegistry(int id)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), id, "Registry id must not be negative.");
        }

        return new RegistryOrInline<T>(id + 1, default);
    }

    public static RegistryOrInline<T> Inline(T value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return new RegistryOrInline<T>(0, value);
    }

    public bool IsInline => _tag == 0;

    public bool IsRegistry => _tag != 0;

    public int Id => _tag != 0
        ? _tag - 1
        : throw new InvalidOperationException("RegistryOrInline holds an inline value, not a registry id.");

    public T Value => _tag == 0 && _value is not null
        ? _value
        : throw new InvalidOperationException("RegistryOrInline holds a registry id, not an inline value.");

    public bool TryGetId(out int id)
    {
        id = _tag - 1;
        return _tag != 0;
    }

    public bool TryGetValue([NotNullWhen(true)] out T? value)
    {
        value = _value;
        return _tag == 0 && _value is not null;
    }

    public static RegistryOrInline<T> Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        var n = reader.ReadVarInt();
        if (n == 0)
        {
            return new RegistryOrInline<T>(0, T.Read(ref reader, protocolVersion));
        }

        if (n < 0)
        {
            throw new InvalidOperationException($"RegistryOrInline read a negative discriminator {n}.");
        }

        return new RegistryOrInline<T>(n, default);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (_tag != 0)
        {
            writer.WriteVarInt(_tag);
            return;
        }

        if (_value is null)
        {
            throw new InvalidOperationException("RegistryOrInline was never initialized: no registry id and no inline value.");
        }

        writer.WriteVarInt(0);
        _value.Write(writer, protocolVersion);
    }
}
