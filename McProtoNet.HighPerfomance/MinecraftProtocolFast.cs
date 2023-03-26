using Org.BouncyCastle.Asn1.BC;
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;

namespace McProtoNet.HighPerfomance
{

    public class MinecraftProtocolFast : IDisposable
    {
        private Stream _baseStream;
        private bool _compressionEnabled;


        private Pipe _pipe;
        private PipeReader _pipeReader;
        private PipeWriter _pipeWriter;
        public MinecraftProtocolFast(Stream baseStream)
        {
            _baseStream = baseStream;
            _pipe = new Pipe();
            _pipeReader = _pipe.Reader;
            _pipeWriter = _pipe.Writer;
        }
        private CancellationTokenSource CTS = new();

        public Task Start()
        {
            Task fill = FillPipeAsync();
            Task read = ReadPipeAsync();

            return Task.WhenAll(fill, read);
        }

        private async Task FillPipeAsync()
        {
            const int minimumBufferSize = 512;

            while (!CTS.IsCancellationRequested)
            {
                Memory<byte> memory = _pipeWriter.GetMemory(minimumBufferSize);
                try
                {
                    int bytesRead = await _baseStream.ReadAsync(memory, CTS.Token);
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
                FlushResult result = await _pipeWriter.FlushAsync(CTS.Token);

                if (result.IsCompleted)
                {
                    break;
                }
            }


        }
        private async Task ReadPipeAsync()
        {
            while (!CTS.IsCancellationRequested)
            {


                ReadResult result = await _pipeReader.ReadAsync(CTS.Token);


                ReadOnlySequence<byte> buffer = result.Buffer;

                //Обработка данных
                //while (true)
                {
                    if (TryReadVarInt(buffer, out int length, out int len))
                    {
                        buffer = buffer.Slice(len);
                        if (buffer.Length < length)
                            break;
                        buffer = ReadPacket(buffer, length);

                    }
                    else
                    {
                        break;
                    }

                }
                _pipeReader.AdvanceTo(buffer.Start, buffer.End);
                // Tell the PipeReader how much of the buffer we have consumed



                // Stop reading if there's no more data coming
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // Mark the PipeReader as complete
            _pipeReader.Complete();
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
        private void ParseData(ReadOnlySequence<byte> data)
        {
            TryReadVarInt(data, out int comprSize, out _);
            //Console.WriteLine("asdasd: " + comprSize);
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
            CTS?.Cancel();
            CTS?.Dispose();
            CTS = null;
            _pipeReader.Complete();
            _pipeWriter.Complete();
            _baseStream?.Dispose();
            _baseStream = null;
            _pipe = null;
            _pipeReader = null;
            _pipeWriter = null;
            GC.SuppressFinalize(this);
        }
    }
}
