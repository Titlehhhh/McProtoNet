using System;
using System.Diagnostics.CodeAnalysis;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

/// <summary>
/// Represents a protocol value that is either a registry id or an inline value of type
/// <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the inline value.</typeparam>
/// <remarks>
/// The wire form is a VarInt discriminator. A discriminator of <c>0</c> means that an inline value
/// follows; any other value is the registry id plus one. The default instance of this type holds
/// neither a registry id nor an inline value.
/// </remarks>
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

    /// <summary>
    /// Creates a value that refers to the specified registry id.
    /// </summary>
    /// <param name="id">The registry id. This value must not be negative.</param>
    /// <returns>A <see cref="RegistryOrInline{T}"/> that holds <paramref name="id"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/> is less than zero.</exception>
    public static RegistryOrInline<T> FromRegistry(int id)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), id, "Registry id must not be negative.");
        }

        return new RegistryOrInline<T>(id + 1, default);
    }

    /// <summary>
    /// Creates a value that carries the specified inline value.
    /// </summary>
    /// <param name="value">The inline value. This value must not be <see langword="null"/>.</param>
    /// <returns>A <see cref="RegistryOrInline{T}"/> that holds <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is
    /// <see langword="null"/>.</exception>
    public static RegistryOrInline<T> Inline(T value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return new RegistryOrInline<T>(0, value);
    }

    /// <summary>
    /// Gets a value indicating whether the current instance carries an inline value.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the discriminator is zero; otherwise, <see langword="false"/>. The
    /// default instance of this type reports <see langword="true"/> although it carries no value.
    /// </value>
    public bool IsInline => _tag == 0;

    /// <summary>
    /// Gets a value indicating whether the current instance carries a registry id.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the current instance carries a registry id; otherwise,
    /// <see langword="false"/>.
    /// </value>
    public bool IsRegistry => _tag != 0;

    /// <summary>
    /// Gets the registry id that the current instance carries.
    /// </summary>
    /// <value>The registry id.</value>
    /// <exception cref="InvalidOperationException">The current instance does not carry a registry
    /// id.</exception>
    public int Id => _tag != 0
        ? _tag - 1
        : throw new InvalidOperationException("RegistryOrInline holds an inline value, not a registry id.");

    /// <summary>
    /// Gets the inline value that the current instance carries.
    /// </summary>
    /// <value>The inline value.</value>
    /// <exception cref="InvalidOperationException">The current instance does not carry an inline
    /// value.</exception>
    public T Value => _tag == 0 && _value is not null
        ? _value
        : throw new InvalidOperationException("RegistryOrInline holds a registry id, not an inline value.");

    /// <summary>
    /// Attempts to get the registry id that the current instance carries.
    /// </summary>
    /// <param name="id">When this method returns, contains the registry id if the current instance
    /// carries one; otherwise, an undefined value.</param>
    /// <returns>
    /// <see langword="true"/> if the current instance carries a registry id; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool TryGetId(out int id)
    {
        id = _tag - 1;
        return _tag != 0;
    }

    /// <summary>
    /// Attempts to get the inline value that the current instance carries.
    /// </summary>
    /// <param name="value">When this method returns, contains the inline value if the current instance
    /// carries one; otherwise, <see langword="null"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the current instance carries an inline value; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool TryGetValue([NotNullWhen(true)] out T? value)
    {
        value = _value;
        return _tag == 0 && _value is not null;
    }

    /// <summary>
    /// Reads a <see cref="RegistryOrInline{T}"/> from the specified reader.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <returns>The value that was read.</returns>
    /// <exception cref="InvalidOperationException">The discriminator read from
    /// <paramref name="reader"/> is negative.</exception>
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

    /// <summary>
    /// Writes the current value to the specified writer.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <exception cref="InvalidOperationException">The current instance carries neither a registry id
    /// nor an inline value.</exception>
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
