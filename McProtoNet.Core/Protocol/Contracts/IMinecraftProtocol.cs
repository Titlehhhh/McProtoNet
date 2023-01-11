using McProtoNet.Core.IO;

namespace McProtoNet.Core.Protocol
{
    public interface IMinecraftProtocol : IDisposable
    {
        bool Available();
        void SwitchEncryption(byte[] privateKey);
        void SwitchCompression(int threshold);
        //Task SendPacketAsync(IOutputPacket packet, int id, CancellationToken token = default);
        // Task<(int, MemoryStream)> ReadNextPacketAsync(CancellationToken token = default);

       // void SendPacket(IOutputPacket packet, int id);
        void SendPacket(MemoryStream data, int id);
        
        Task SendPacketAsync(MemoryStream memoryStream, int id, CancellationToken cancellationToken = default);
        
        //void SendPacket(MemoryStream data, int id);
        //void SendPacket(Span<byte> data, int id);
        (int, MemoryStream) ReadNextPacket();
        Task<(int, MemoryStream)> ReadNextPacketAsync(CancellationToken cancellationToken = default);
    }
}

