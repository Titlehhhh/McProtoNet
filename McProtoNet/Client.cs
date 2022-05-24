using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Packets;
using McProtoNet.API.Protocol;
using McProtoNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet
{
    public delegate void ConnectedHandler(IClient client);
    public delegate void DisconnectedHandler(IClient client, string reason);
    public delegate void ErrorHandler(IClient client, Exception exception);
    public delegate void PacketReceivedHandler(IClient client, Packet packet);
    public delegate void PacketSentHandler(IClient client, Packet packet);
    public delegate void PacketSendHandler(IClient client, Packet packet);

    public abstract class Client : IClient
    {

        protected TcpClient tcpClient;
        protected IPacketReaderWriter packetReaderWriter;
        protected ISessionCheckService sessionCheckService;

        protected IPacketDictionary packetDictionary;

        protected Task MainTask;

        protected string _nick;
        protected string _uuid;
        protected string _sessionId;

        protected CancellationTokenSource CancellationTokenSource;
        public Client(TcpClient tcpClient, SessionToken session, IPacketDictionary packetDictionary, ISessionCheckService sessionCheckService)
        {
            ArgumentNullException.ThrowIfNull(tcpClient, nameof(tcpClient));
            ArgumentNullException.ThrowIfNull(packetDictionary, nameof(packetDictionary));
            ArgumentNullException.ThrowIfNull(session, nameof(session));
            ArgumentNullException.ThrowIfNull(sessionCheckService, nameof(sessionCheckService));

            this.packetDictionary = packetDictionary;
            _nick = session.Username;
            _uuid = session.UUID;
            _sessionId = session.SessionId;

            CancellationTokenSource = new();
            this.tcpClient = tcpClient;
            packetReaderWriter = new PacketReaderWriter(tcpClient);



            MainTask =
                Task.Factory.StartNew(MainAction,
                TaskCreationOptions.LongRunning);
            this.sessionCheckService = sessionCheckService;
        }

        public bool Connected => tcpClient.Connected;


        public event DisconnectedHandler OnDisconnected;
        public event PacketReceivedHandler OnPacketReceived;
        public event PacketSentHandler OnPacketSent;
        public event PacketSendHandler OnPacketSend;
        public event DisconnectedHandler OnLoginRejected;
        public event ErrorHandler OnError;

        public virtual async Task DisconnectAsync()
        {
            CancellationTokenSource.Cancel();
            tcpClient.Close();
            await MainTask;
        }

        protected abstract bool CheckPacket(Packet packet);

        protected async virtual void MainAction()
        {
            try
            {
                while (tcpClient.Connected && !CancellationTokenSource.IsCancellationRequested)
                {

                    (int id, MemoryStream data) =
                        await packetReaderWriter.ReadNextPacketAsync(CancellationTokenSource.Token);
                    if (packetDictionary.TryGetInputPacket(id, out Lazy<Packet> lazy))
                    {
                        Packet packet = lazy.Value;
                        IMinecraftPrimitiveReader primitiveReader = new MinecraftPrimitiveReader(data);
                        packet.Read(primitiveReader);
                        if (!CheckPacket(packet))
                        {
                            return;
                        }

                        PacketReceivedEvent(packet);
                    }

                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                ErrorEvent(e);
            }
            finally
            {
                tcpClient.Close();
            }
            OnDisconnected?.Invoke(this, "");
        }

        public virtual void Dispose()
        {
            tcpClient.Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual async Task SendPacketAsync(Packet packet)
        {
            try
            {
                if (packetDictionary.TryGetOutputId(packet.GetType(), out int id))
                {
                    OnPacketSend?.Invoke(this, packet);
                    await packetReaderWriter.SendPacketAsync(packet, id, CancellationTokenSource.Token);
                    OnPacketSent?.Invoke(this, packet);
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch
            {
                throw;
            }
        }

        protected void DisconnectedEvent(string reason)
        {
            OnDisconnected?.Invoke(this, reason);

        }
        protected void ErrorEvent(Exception e)
        {
            OnError?.Invoke(this, e);
        }
        protected void PacketReceivedEvent(Packet packet)
        {
            OnPacketReceived?.Invoke(this, packet);
        }
        protected void PacketSentEvent(Packet packet)
        {
            OnPacketSent?.Invoke(this, packet);
        }
        protected void PacketSendEvent(Packet packet)
        {
            OnPacketSend?.Invoke(this, packet);
        }
        protected void LoginRejectedEvent(string reason)
        {
            OnLoginRejected?.Invoke(this, reason);
        }
    }
}
