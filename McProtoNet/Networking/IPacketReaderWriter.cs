using McProtoNet.IO;

namespace McProtoNet.Networking
{
    public interface IPacketReaderWriter : IDisposable
    {
        void SwitchEncryption(byte[] privateKey);
        void SwitchCompression(int threshold);
        Task SendPacketAsync(IPacket packet, int id, CancellationToken token = default);
        Task<(int, MemoryStream)> ReadNextPacketAsync(CancellationToken token = default);
    }
}
