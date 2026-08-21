using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Transport.Compression;
using McProtoNet.Primitives;
namespace McProtoNet.Transport.Framing;

public static class PacketWriteExtensions
{
    private const int MaxHeaderLength = 16;

    #region IBufferWriter

    public static void WritePacket(this IBufferWriter<byte> writer, ReadOnlySpan<byte> packet,
        int compressionThreshold = -1)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (compressionThreshold < 0)
        {
            writer.WriteVarInt(packet.Length);
            writer.Write(packet);
            return;
        }

        if (packet.Length < compressionThreshold)
        {
            writer.WriteVarInt(packet.Length + 1);
            writer.WriteVarInt(0);
            writer.Write(packet);
            return;
        }

        WriteCompressed(writer, packet);
    }

    public static void WritePacket(this IBufferWriter<byte> writer, int id, ReadOnlySpan<byte> body,
        int compressionThreshold = -1)
    {
        ArgumentNullException.ThrowIfNull(writer);

        int idLength = id.GetVarIntLength();
        int packetLength = idLength + body.Length;

        if (compressionThreshold < 0)
        {
            writer.WriteVarInt(packetLength);
            writer.WriteVarInt(id);
            writer.Write(body);
            return;
        }

        if (packetLength < compressionThreshold)
        {
            writer.WriteVarInt(packetLength + 1);
            writer.WriteVarInt(0);
            writer.WriteVarInt(id);
            writer.Write(body);
            return;
        }

        byte[] joined = ArrayPool<byte>.Shared.Rent(packetLength);
        try
        {
            id.GetVarIntLength(joined.AsSpan(0, idLength));
            body.CopyTo(joined.AsSpan(idLength));
            WriteCompressed(writer, joined.AsSpan(0, packetLength));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(joined);
        }
    }

    public static void WritePacket(this IBufferWriter<byte> writer, in ReadOnlySequence<byte> packet,
        int compressionThreshold = -1)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (packet.IsSingleSegment)
        {
            writer.WritePacket(packet.FirstSpan, compressionThreshold);
            return;
        }

        int packetLength = checked((int)packet.Length);

        if (compressionThreshold < 0)
        {
            writer.WriteVarInt(packetLength);
            WriteSegments(writer, in packet);
            return;
        }

        if (packetLength < compressionThreshold)
        {
            writer.WriteVarInt(packetLength + 1);
            writer.WriteVarInt(0);
            WriteSegments(writer, in packet);
            return;
        }

        byte[] joined = ArrayPool<byte>.Shared.Rent(packetLength);
        try
        {
            packet.CopyTo(joined);
            WriteCompressed(writer, joined.AsSpan(0, packetLength));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(joined);
        }
    }

    public static void WritePacket(this IBufferWriter<byte> writer, int id, in ReadOnlySequence<byte> body,
        int compressionThreshold = -1)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (body.IsSingleSegment)
        {
            writer.WritePacket(id, body.FirstSpan, compressionThreshold);
            return;
        }

        int idLength = id.GetVarIntLength();
        int packetLength = checked((int)body.Length + idLength);

        if (compressionThreshold < 0)
        {
            writer.WriteVarInt(packetLength);
            writer.WriteVarInt(id);
            WriteSegments(writer, in body);
            return;
        }

        if (packetLength < compressionThreshold)
        {
            writer.WriteVarInt(packetLength + 1);
            writer.WriteVarInt(0);
            writer.WriteVarInt(id);
            WriteSegments(writer, in body);
            return;
        }

        byte[] joined = ArrayPool<byte>.Shared.Rent(packetLength);
        try
        {
            id.GetVarIntLength(joined.AsSpan(0, idLength));
            body.CopyTo(joined.AsSpan(idLength));
            WriteCompressed(writer, joined.AsSpan(0, packetLength));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(joined);
        }
    }

    #endregion

    #region PipeWriter

    public static ValueTask<FlushResult> WritePacketAsync(this PipeWriter writer, ReadOnlyMemory<byte> packet,
        int compressionThreshold = -1, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(writer);

        writer.WritePacket(packet.Span, compressionThreshold);
        return writer.FlushAsync(cancellationToken);
    }

    public static ValueTask<FlushResult> WritePacketAsync(this PipeWriter writer, int id, ReadOnlyMemory<byte> body,
        int compressionThreshold = -1, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(writer);

        writer.WritePacket(id, body.Span, compressionThreshold);
        return writer.FlushAsync(cancellationToken);
    }

    public static ValueTask<FlushResult> WritePacketAsync(this PipeWriter writer, ReadOnlySequence<byte> packet,
        int compressionThreshold = -1, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(writer);

        writer.WritePacket(in packet, compressionThreshold);
        return writer.FlushAsync(cancellationToken);
    }

    public static ValueTask<FlushResult> WritePacketAsync(this PipeWriter writer, int id, ReadOnlySequence<byte> body,
        int compressionThreshold = -1, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(writer);

        writer.WritePacket(id, in body, compressionThreshold);
        return writer.FlushAsync(cancellationToken);
    }

    #endregion

    #region Stream

    public static void WritePacket(this Stream stream, ReadOnlySpan<byte> packet, int compressionThreshold = -1)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (ShouldCompress(packet.Length, compressionThreshold))
        {
            WriteCompressed(stream, packet);
            return;
        }

        Span<byte> header = stackalloc byte[MaxHeaderLength];
        int headerLength = WritePlainHeader(header, packet.Length, compressionThreshold);
        stream.Write(header[..headerLength]);
        stream.Write(packet);
    }

    public static void WritePacket(this Stream stream, int id, ReadOnlySpan<byte> body,
        int compressionThreshold = -1)
    {
        ArgumentNullException.ThrowIfNull(stream);

        int idLength = id.GetVarIntLength();
        int packetLength = idLength + body.Length;

        if (ShouldCompress(packetLength, compressionThreshold))
        {
            byte[] joined = ArrayPool<byte>.Shared.Rent(packetLength);
            try
            {
                id.GetVarIntLength(joined.AsSpan(0, idLength));
                body.CopyTo(joined.AsSpan(idLength));
                WriteCompressed(stream, joined.AsSpan(0, packetLength));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(joined);
            }

            return;
        }

        Span<byte> header = stackalloc byte[MaxHeaderLength];
        int headerLength = WritePlainHeader(header, packetLength, compressionThreshold);
        headerLength += id.GetVarIntLength(header[headerLength..]);
        stream.Write(header[..headerLength]);
        stream.Write(body);
    }

    public static void WritePacket(this Stream stream, in ReadOnlySequence<byte> packet,
        int compressionThreshold = -1)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (packet.IsSingleSegment)
        {
            stream.WritePacket(packet.FirstSpan, compressionThreshold);
            return;
        }

        int packetLength = checked((int)packet.Length);

        if (ShouldCompress(packetLength, compressionThreshold))
        {
            byte[] joined = ArrayPool<byte>.Shared.Rent(packetLength);
            try
            {
                packet.CopyTo(joined);
                WriteCompressed(stream, joined.AsSpan(0, packetLength));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(joined);
            }

            return;
        }

        Span<byte> header = stackalloc byte[MaxHeaderLength];
        int headerLength = WritePlainHeader(header, packetLength, compressionThreshold);
        stream.Write(header[..headerLength]);

        foreach (ReadOnlyMemory<byte> segment in packet)
        {
            stream.Write(segment.Span);
        }
    }

    public static void WritePacket(this Stream stream, int id, in ReadOnlySequence<byte> body,
        int compressionThreshold = -1)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (body.IsSingleSegment)
        {
            stream.WritePacket(id, body.FirstSpan, compressionThreshold);
            return;
        }

        int idLength = id.GetVarIntLength();
        int packetLength = checked((int)body.Length + idLength);

        if (ShouldCompress(packetLength, compressionThreshold))
        {
            byte[] joined = ArrayPool<byte>.Shared.Rent(packetLength);
            try
            {
                id.GetVarIntLength(joined.AsSpan(0, idLength));
                body.CopyTo(joined.AsSpan(idLength));
                WriteCompressed(stream, joined.AsSpan(0, packetLength));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(joined);
            }

            return;
        }

        Span<byte> header = stackalloc byte[MaxHeaderLength];
        int headerLength = WritePlainHeader(header, packetLength, compressionThreshold);
        headerLength += id.GetVarIntLength(header[headerLength..]);
        stream.Write(header[..headerLength]);

        foreach (ReadOnlyMemory<byte> segment in body)
        {
            stream.Write(segment.Span);
        }
    }

    #endregion

    #region Stream async

    public static ValueTask WritePacketAsync(this Stream stream, ReadOnlyMemory<byte> packet,
        int compressionThreshold = -1, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return WriteMemoryCoreAsync(stream, packet, compressionThreshold, cancellationToken);
    }

    public static ValueTask WritePacketAsync(this Stream stream, int id, ReadOnlyMemory<byte> body,
        int compressionThreshold = -1, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return WriteMemoryCoreAsync(stream, id, body, compressionThreshold, cancellationToken);
    }

    public static ValueTask WritePacketAsync(this Stream stream, ReadOnlySequence<byte> packet,
        int compressionThreshold = -1, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return WriteSequenceCoreAsync(stream, packet, compressionThreshold, cancellationToken);
    }

    public static ValueTask WritePacketAsync(this Stream stream, int id, ReadOnlySequence<byte> body,
        int compressionThreshold = -1, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return WriteSequenceCoreAsync(stream, id, body, compressionThreshold, cancellationToken);
    }

    #endregion

    #region Stream core

    private static async ValueTask WriteMemoryCoreAsync(Stream stream, ReadOnlyMemory<byte> packet,
        int compressionThreshold, CancellationToken cancellationToken)
    {
        if (ShouldCompress(packet.Length, compressionThreshold))
        {
            await WriteCompressedAsync(stream, packet, cancellationToken).ConfigureAwait(false);
            return;
        }

        byte[] header = ArrayPool<byte>.Shared.Rent(MaxHeaderLength);
        try
        {
            int headerLength = WritePlainHeader(header, packet.Length, compressionThreshold);
            await stream.WriteAsync(header.AsMemory(0, headerLength), cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(packet, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(header);
        }
    }

    private static async ValueTask WriteMemoryCoreAsync(Stream stream, int id, ReadOnlyMemory<byte> body,
        int compressionThreshold, CancellationToken cancellationToken)
    {
        int idLength = id.GetVarIntLength();
        int packetLength = idLength + body.Length;

        if (ShouldCompress(packetLength, compressionThreshold))
        {
            await WriteCompressedAsync(stream, id, body, cancellationToken).ConfigureAwait(false);
            return;
        }

        byte[] header = ArrayPool<byte>.Shared.Rent(MaxHeaderLength);
        try
        {
            int headerLength = WritePlainHeader(header, packetLength, compressionThreshold);
            headerLength += id.GetVarIntLength(header.AsSpan(headerLength));
            await stream.WriteAsync(header.AsMemory(0, headerLength), cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(body, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(header);
        }
    }

    private static async ValueTask WriteSequenceCoreAsync(Stream stream, ReadOnlySequence<byte> packet,
        int compressionThreshold, CancellationToken cancellationToken)
    {
        if (packet.IsSingleSegment)
        {
            await WriteMemoryCoreAsync(stream, packet.First, compressionThreshold, cancellationToken)
                .ConfigureAwait(false);
            return;
        }

        int packetLength = checked((int)packet.Length);

        if (ShouldCompress(packetLength, compressionThreshold))
        {
            await WriteCompressedAsync(stream, packet, packetLength, cancellationToken).ConfigureAwait(false);
            return;
        }

        byte[] header = ArrayPool<byte>.Shared.Rent(MaxHeaderLength);
        try
        {
            int headerLength = WritePlainHeader(header, packetLength, compressionThreshold);
            await stream.WriteAsync(header.AsMemory(0, headerLength), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(header);
        }

        foreach (ReadOnlyMemory<byte> segment in packet)
        {
            await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async ValueTask WriteSequenceCoreAsync(Stream stream, int id, ReadOnlySequence<byte> body,
        int compressionThreshold, CancellationToken cancellationToken)
    {
        if (body.IsSingleSegment)
        {
            await WriteMemoryCoreAsync(stream, id, body.First, compressionThreshold, cancellationToken)
                .ConfigureAwait(false);
            return;
        }

        int idLength = id.GetVarIntLength();
        int packetLength = checked((int)body.Length + idLength);

        if (ShouldCompress(packetLength, compressionThreshold))
        {
            await WriteCompressedAsync(stream, id, body, packetLength, idLength, cancellationToken)
                .ConfigureAwait(false);
            return;
        }

        byte[] header = ArrayPool<byte>.Shared.Rent(MaxHeaderLength);
        try
        {
            int headerLength = WritePlainHeader(header, packetLength, compressionThreshold);
            headerLength += id.GetVarIntLength(header.AsSpan(headerLength));
            await stream.WriteAsync(header.AsMemory(0, headerLength), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(header);
        }

        foreach (ReadOnlyMemory<byte> segment in body)
        {
            await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async ValueTask WriteCompressedAsync(Stream stream, ReadOnlyMemory<byte> packet,
        CancellationToken cancellationToken)
    {
        byte[] frame = RentCompressedFrame(packet.Span, out int start, out int length);
        try
        {
            await stream.WriteAsync(frame.AsMemory(start, length), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(frame);
        }
    }

    private static async ValueTask WriteCompressedAsync(Stream stream, int id, ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken)
    {
        int idLength = id.GetVarIntLength();
        byte[] joined = ArrayPool<byte>.Shared.Rent(idLength + body.Length);
        try
        {
            id.GetVarIntLength(joined.AsSpan(0, idLength));
            body.Span.CopyTo(joined.AsSpan(idLength));
            await WriteCompressedAsync(stream, joined.AsMemory(0, idLength + body.Length), cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(joined);
        }
    }

    private static async ValueTask WriteCompressedAsync(Stream stream, ReadOnlySequence<byte> packet,
        int packetLength, CancellationToken cancellationToken)
    {
        byte[] joined = ArrayPool<byte>.Shared.Rent(packetLength);
        try
        {
            packet.CopyTo(joined);
            await WriteCompressedAsync(stream, joined.AsMemory(0, packetLength), cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(joined);
        }
    }

    private static async ValueTask WriteCompressedAsync(Stream stream, int id, ReadOnlySequence<byte> body,
        int packetLength, int idLength, CancellationToken cancellationToken)
    {
        byte[] joined = ArrayPool<byte>.Shared.Rent(packetLength);
        try
        {
            id.GetVarIntLength(joined.AsSpan(0, idLength));
            body.CopyTo(joined.AsSpan(idLength));
            await WriteCompressedAsync(stream, joined.AsMemory(0, packetLength), cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(joined);
        }
    }

    #endregion

    #region Framing

    private static bool ShouldCompress(int packetLength, int compressionThreshold)
    {
        return compressionThreshold >= 0 && packetLength >= compressionThreshold;
    }

    private static int WritePlainHeader(Span<byte> header, int packetLength, int compressionThreshold)
    {
        if (compressionThreshold < 0)
        {
            return packetLength.GetVarIntLength(header);
        }

        int written = (packetLength + 1).GetVarIntLength(header);
        header[written++] = 0;
        return written;
    }

    private static void WriteSegments(IBufferWriter<byte> writer, in ReadOnlySequence<byte> value)
    {
        foreach (ReadOnlyMemory<byte> segment in value)
        {
            writer.Write(segment.Span);
        }
    }

    private static void WriteCompressed(IBufferWriter<byte> writer, ReadOnlySpan<byte> packet)
    {
        int uncompressedSize = packet.Length;
        var compressor = LibDeflateCache.RentCompressor();
        int bound = compressor.GetBound(uncompressedSize);

        byte[] rented = ArrayPool<byte>.Shared.Rent(bound);
        try
        {
            int compressedLength = compressor.Compress(packet, rented.AsSpan(0, bound));

            writer.WriteVarInt(compressedLength + uncompressedSize.GetVarIntLength());
            writer.WriteVarInt(uncompressedSize);
            writer.Write(rented.AsSpan(0, compressedLength));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    private static void WriteCompressed(Stream stream, ReadOnlySpan<byte> packet)
    {
        byte[] frame = RentCompressedFrame(packet, out int start, out int length);
        try
        {
            stream.Write(frame.AsSpan(start, length));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(frame);
        }
    }

    private static byte[] RentCompressedFrame(ReadOnlySpan<byte> packet, out int start, out int length)
    {
        int uncompressedSize = packet.Length;
        var compressor = LibDeflateCache.RentCompressor();
        int bound = compressor.GetBound(uncompressedSize);

        byte[] frame = ArrayPool<byte>.Shared.Rent(MaxHeaderLength + bound);
        try
        {
            int compressedLength = compressor.Compress(packet, frame.AsSpan(MaxHeaderLength));

            int sizeLength = uncompressedSize.GetVarIntLength();
            int fullSize = compressedLength + sizeLength;
            int headerLength = fullSize.GetVarIntLength() + sizeLength;

            start = MaxHeaderLength - headerLength;
            Span<byte> header = frame.AsSpan(start, headerLength);
            int written = fullSize.GetVarIntLength(header);
            uncompressedSize.GetVarIntLength(header[written..]);

            length = headerLength + compressedLength;
            return frame;
        }
        catch
        {
            ArrayPool<byte>.Shared.Return(frame);
            throw;
        }
    }

    #endregion
}
