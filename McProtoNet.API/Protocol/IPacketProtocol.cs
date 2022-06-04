﻿namespace McProtoNet.API.Protocol
{
    public interface IPacketProtocol : IDisposable
    {
        bool Available();
        void SwitchEncryption(byte[] privateKey);
        void SwitchCompression(int threshold);
        Task SendPacketAsync(Packet packet, int id, CancellationToken token = default);
        Task<(int, MemoryStream)> ReadNextPacketAsync(CancellationToken token = default);
    }
}