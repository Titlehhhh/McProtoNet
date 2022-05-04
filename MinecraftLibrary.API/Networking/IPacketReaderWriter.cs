﻿using ProtoLib.API.IO;

namespace ProtoLib.API.Networking
{
    public interface IPacketReaderWriter : IDisposable
    {
        NetworkMinecraftStream NetStream { get; }
        int CompressionThreshold { set; }
        Task WritePacketAsync(IPacket packet, int id, CancellationToken token = default);
        Task<(int, MinecraftStream)> ReadNextPacketAsync(CancellationToken token = default);
    }
}
