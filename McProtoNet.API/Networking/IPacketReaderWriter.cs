using McProtoNet.API.IO;

namespace McProtoNet.API.Networking
{
    public interface IPacketReaderWriter : IDisposable
    {
        void SwitchEncryption(byte[] privateKey);
        void SwitchCompression(int threshold);
        Task SendPacketAsync(IPacket packet, int id, CancellationToken token = default);
        Task<(int, MinecraftStream)> ReadNextPacketAsync(CancellationToken token = default);
    }
}
