namespace McProtoNet.Core.Protocol
{
    public interface IPacketProtocol : IDisposable, IAsyncDisposable
    {
        bool Available();
        void SwitchEncryption(byte[] privateKey);
        void SwitchCompression(int threshold);
        Task SendPacketAsync(Packet packet, int id, CancellationToken token = default);
        Task<(int, MemoryStream)> ReadNextPacketAsync(CancellationToken token = default);

        void SendPacket(Packet packet, int id);
        void SendPacket(byte[] data, int id);
        void SendPacket(MemoryStream data, int id);
        void SendPacket(Span<byte> data, int id);
        (int, MemoryStream) ReadNextPacket();
    }
}
