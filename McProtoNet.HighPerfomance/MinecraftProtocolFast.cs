using McProtoNet.Core.IO;
using McProtoNet.Core.Protocol;
using Microsoft.IO;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Threading;

namespace McProtoNet.HighPerfomance
{
    public class MinecraftProtocolFast : IDisposable, IAsyncDisposable
    {

        private static readonly RecyclableMemoryStreamManager MSmanager = new RecyclableMemoryStreamManager();

        private Stream _baseStream;
        private int _compressionThreshold;

        public event Action<Packet> OnReceived;

        private PipeReader _pipeReader;
        private PipeWriter _pipeWriter;

        public MinecraftProtocolFast(Stream baseStream, PipeReader pipeReader, PipeWriter pipeWriter, int compressionThreshold)
        {
            _baseStream = baseStream;
            _pipeReader = pipeReader;
            _pipeWriter = pipeWriter;
            _compressionThreshold = compressionThreshold;
        }
        public MinecraftProtocolFast(Stream baseStream, int compressionThreshold)
        {
            Pipe pipe = new Pipe();

            _baseStream = baseStream;
            _pipeReader = pipe.Reader;
            _pipeWriter = pipe.Writer;
            _compressionThreshold = compressionThreshold;
        }



        public Task Start(CancellationToken cancellationToken)
        {
            Task fill = FillPipeAsync(cancellationToken);
            Task read = ReadPipeAsync(cancellationToken);

            return Task.WhenAll(fill, read);
        }

        private async Task FillPipeAsync(CancellationToken cancellationToken)
        {
            const int minimumBufferSize = 512;

            while (!cancellationToken.IsCancellationRequested)
            {
                Memory<byte> memory = _pipeWriter.GetMemory(minimumBufferSize);
                try
                {
                    int bytesRead = await _baseStream.ReadAsync(memory, cancellationToken);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    // Tell the PipeWriter how much was read from the Socket
                    _pipeWriter.Advance(bytesRead);
                }
                catch (Exception ex)
                {
                    break;
                }
                // Make the data available to the PipeReader
                FlushResult result = await _pipeWriter.FlushAsync(cancellationToken);

                if (result.IsCompleted)
                {
                    break;
                }
            }


            await _pipeWriter.CompleteAsync();

        }
        private ZLibStream ReadZlib;
        private async Task ReadPipeAsync(CancellationToken cancellationToken)
        {
            ReadZlib = new ZLibStream(_pipeReader.AsStream(), CompressionMode.Decompress);

            Stream stream = _pipeReader.AsStream();

            int debugCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var d = await ReadNextPacketAsync(stream, cancellationToken);
                OnReceived?.Invoke(d);


            }
            await _pipeReader.CompleteAsync();


        }

        public async Task<Packet> ReadNextPacketAsync(Stream stream, CancellationToken token)
        {
            //ThrowIfDisposed();

            int len = await stream.ReadVarIntAsync(token);
            if (_compressionThreshold <= 0)
            {

                int id = await stream.ReadVarIntAsync(token);
                len -= id.GetVarIntLength();
                var data = ArrayPool<byte>.Shared.Rent(len);
                try
                {
                    // byte[] data = new byte[len];
                    await stream.ReadToEndAsync(data, len, token);

                    return new Packet(id, MSmanager.GetStream(data.AsSpan(0, len)));
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }
            }

            int sizeUncompressed = await stream.ReadVarIntAsync(token);
            if (sizeUncompressed > 0)
            {

                len -= sizeUncompressed.GetVarIntLength();
                var data = ArrayPool<byte>.Shared.Rent(sizeUncompressed);
                try
                {
                    int id = await ReadZlib.ReadVarIntAsync(token);

                    sizeUncompressed -= id.GetVarIntLength();


                    await ReadZlib.ReadAsync(data);


                    return new Packet(id, MSmanager.GetStream(data.AsSpan(0, sizeUncompressed)));

                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(data);
                }


            }

            {

                int id = await stream.ReadVarIntAsync(token);
                len -= id.GetVarIntLength() + 1;


                byte[] buffer = new byte[len];
                await stream.ReadToEndAsync(buffer, len, token);
                return new Packet(id, new MemoryStream(buffer));
            }

        }


        private ReadOnlySequence<byte> ReadPacket(ReadOnlySequence<byte> buffer, int len)
        {
            SequenceReader<byte> reader = new SequenceReader<byte>(buffer);
            reader.TryRead(out var data);

            //ParseData(new ReadOnlySequence<byte>());
            //TryReadVarInt(reader, out int id, out _);

            // Console.WriteLine("Read id: " + id);

            return buffer.Slice(len);
        }

        private bool TryReadPacket(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> packet)
        {
            if (TryReadVarInt(buffer, out int len, out int varintLen))
            {
                buffer = buffer.Slice(varintLen);
                packet = buffer.Slice(0, len);
                buffer = buffer.Slice(len);
                return true;
            }
            else
            {
                packet = default;
                return false;
            }
        }

        private bool TryReadVarInt(ReadOnlySequence<byte> buffer, out int result, out int len)
        {
            SequenceReader<byte> reader = new SequenceReader<byte>(buffer);
            return TryReadVarInt(reader, out result, out len);

        }
        private bool TryReadVarInt(SequenceReader<byte> reader, out int result, out int len)
        {


            byte read;
            result = 0;
            len = 0;
            do
            {
                if (!reader.TryRead(out read))
                {
                    result = 0;
                    len = 0;
                    return false;
                }
                // await stream.ReadAsync(buff, token);
                // read = buff[0];
                int value = read & 0b01111111;
                result |= value << 7 * len;

                len++;
                if (len > 5)
                {
                    return false;

                }
            } while ((read & 0b10000000) != 0);

            return true;
        }
        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            _pipeReader.Complete();
            _pipeWriter.Complete();
            _baseStream?.Dispose();
            _baseStream = null;
            // _pipe = null;
            _pipeReader = null;
            _pipeWriter = null;
            GC.SuppressFinalize(this);
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
