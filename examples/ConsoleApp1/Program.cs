using System.Net.Sockets;
using System.Threading.Channels;
using McProtoNet.Abstractions;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

IServerPacket packet = PacketFactory.CreateClientboundPacket(769, 0x00, PacketState.Play);