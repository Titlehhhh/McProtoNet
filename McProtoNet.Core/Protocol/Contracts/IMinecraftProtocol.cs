﻿using System.Buffers;
using System.IO.Compression;

namespace McProtoNet.Core.Protocol
{
    public interface IMinecraftProtocol : IMinecraftPacketSender, IMinecraftPacketReader, IDisposable, IAsyncDisposable, ISwitchCompression
    {
       
    }
}

