using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Threading;

namespace McProtoNet.HighPerfomance
{

    public class MinecraftProtocolFast : IDisposable, IAsyncDisposable
    {
        private Stream _baseStream;
        private bool _compressionEnabled;

        public event Action<ReadOnlySequence<byte>> OnReceived;

        private PipeReader _pipeReader;
        private PipeWriter _pipeWriter;

        public MinecraftProtocolFast(Stream baseStream, PipeReader pipeReader, PipeWriter pipeWriter)
        {
            _baseStream = baseStream;
            _pipeReader = pipeReader;
            _pipeWriter = pipeWriter;
        }
        public MinecraftProtocolFast(Stream baseStream)
        {
            Pipe pipe = new Pipe();
            _baseStream = baseStream;
            _pipeReader = pipe.Reader;
            _pipeWriter = pipe.Writer;
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
        private async Task ReadPipeAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {


                ReadResult result = await _pipeReader.ReadAsync(cancellationToken);


                ReadOnlySequence<byte> buffer = result.Buffer;
                buffer = buffer.Slice(0);
                if (false)
                    while (TryReadPacket(ref buffer, out var packet))
                    {
                        this?.OnReceived(packet);
                    }

                _pipeReader.AdvanceTo(buffer.Start, buffer.End);
                // Tell the PipeReader how much of the buffer we have consumed



                // Stop reading if there's no more data coming
                if (result.IsCompleted)
                {
                    break;
                }
            }


            await _pipeReader.CompleteAsync();
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
