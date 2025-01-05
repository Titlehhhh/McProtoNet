using System.Net.Sockets;
using McProtoNet;
using McProtoNet.Abstractions;
using McProtoNet.Net;

internal class Client : IDisposable
{
    private TcpClient _tcpClient;
    private MinecraftPacketReader _reader;
    private MinecraftPacketSender _sender;
    private SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    public Stream MainStream => _tcpClient.GetStream();

    public Client(TcpClient tcpClient, MinecraftPacketReader reader, MinecraftPacketSender sender)
    {
        _tcpClient = tcpClient;
        _reader = reader;
        _sender = sender;
    }


    public async Task<InputPacket> ReadNextPacketAsync()
    {
        return await _reader.ReadNextPacketAsync();
    }

    public async Task SendPacketAsync(OutputPacket packet)
    {
        await _lock.WaitAsync();
        try
        {
            await _sender.SendAndDisposeAsync(packet, default);
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose()
    {
        _tcpClient.Dispose();
        _lock.Dispose();
    }
}