using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Packets.Play.Clientbound;

namespace McProtoNet.Benchmark;

[MemoryDiagnoser]
public class CreatePacketBenchmarks
{
    [GlobalSetup]
    public void Setup()
    {
        PacketFactory.Init();
        var dict = new Dictionary<int, Type>();
        
        for (int i1 = 0; i1 < ServerPacketRegistry.Packets.Length; i1++)
        {
            dict[i1] = ServerPacketRegistry.Packets[i1]().GetType();
        }

        _types = dict.ToFrozenDictionary();
        
        List<int> ids = new();
        List<Type> types = new();
        int i = 0;
        while (ids.Count < 10)
        {
            try
            {
                IServerPacket packet = PacketFactory.CreateClientboundPacket(340,i , PacketState.Play);
                ids.Add(i);
                types.Add(packet.GetType());
                
            }
            catch
            {
                // ignored
            }
            i++;
        }

        _ids1 = ids.ToArray();

        _ids2 = types.Select(t =>
        {
            for (int j = 0; j < ServerPacketRegistry.Packets.Length; j++)
            {
                var func = ServerPacketRegistry.Packets[j];
                IServerPacket packet = func();
                if (packet.GetType() == t)
                {
                    return j;
                }
            }

            throw new Exception();
        }).ToArray();
        
        
    }

    private readonly Random _r = new(504);

    private static int[] _ids1;
    private static int[] _ids2;

    private FrozenDictionary<int, Type> _types;


    [Benchmark]
    public void CreatePacket()
    {
        //0x08 - BlockAction
        for (int i = 0; i <= 1000; i++)
        {
            int id = _ids1[_r.Next(_ids1.Length)];
            
            IServerPacket packet = PacketFactory.CreateClientboundPacket(340, id, PacketState.Play);
        }
    }

    [Benchmark]
    public void CreatePacketReflection()
    {
        for (int i = 0; i < 1000; i++)
        {
            int id = _ids2[_r.Next(_ids2.Length)];
            IServerPacket packet = (IServerPacket)Activator.CreateInstance(_types[id]);
        }
    }
}