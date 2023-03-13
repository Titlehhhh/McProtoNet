using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Buffers.Binary;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace QuickProxy
{
    public class Socks4ProxyClient : IProxyClient
    {
        private const int WAIT_FOR_DATA_INTERVAL = 50;   // 50 ms
        private const int WAIT_FOR_DATA_TIMEOUT = 15000; // 15 seconds
        private const string PROXY_NAME = "SOCKS4";
        private TcpClient _tcpClient;

        private string _proxyHost;
        private ushort _proxyPort;
        private string _proxyUserId;


        internal const int SOCKS_PROXY_DEFAULT_PORT = 1080;

        internal const byte SOCKS4_VERSION_NUMBER = 4;

        internal const byte SOCKS4_CMD_CONNECT = 0x01;

        internal const byte SOCKS4_CMD_BIND = 0x02;

        internal const byte SOCKS4_CMD_REPLY_REQUEST_GRANTED = 90;

        internal const byte SOCKS4_CMD_REPLY_REQUEST_REJECTED_OR_FAILED = 91;

        internal const byte SOCKS4_CMD_REPLY_REQUEST_REJECTED_CANNOT_CONNECT_TO_IDENTD = 92;

        internal const byte SOCKS4_CMD_REPLY_REQUEST_REJECTED_DIFFERENT_IDENTD = 93;


        public Socks4ProxyClient() { }


        public Socks4ProxyClient(TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");

            _tcpClient = tcpClient;
        }


        public Socks4ProxyClient(string proxyHost, string proxyUserId)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            if (proxyUserId == null)
                throw new ArgumentNullException("proxyUserId");

            _proxyHost = proxyHost;
            _proxyPort = SOCKS_PROXY_DEFAULT_PORT;
            _proxyUserId = proxyUserId;
        }

        public Socks4ProxyClient(string proxyHost, ushort proxyPort, string proxyUserId)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            if (proxyPort <= 0 || proxyPort > 65535)
                throw new ArgumentOutOfRangeException("proxyPort", "port must be greater than zero and less than 65535");

            if (proxyUserId == null)
                throw new ArgumentNullException("proxyUserId");

            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
            _proxyUserId = proxyUserId;
        }


        public Socks4ProxyClient(string proxyHost)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            _proxyHost = proxyHost;
            _proxyPort = SOCKS_PROXY_DEFAULT_PORT;
        }

        public Socks4ProxyClient(string proxyHost, ushort proxyPort)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            if (proxyPort <= 0 || proxyPort > 65535)
                throw new ArgumentOutOfRangeException("proxyPort", "port must be greater than zero and less than 65535");

            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
        }

        public string ProxyHost
        {
            get { return _proxyHost; }
            set { _proxyHost = value; }
        }
        public ushort ProxyPort
        {
            get { return _proxyPort; }
            set { _proxyPort = value; }
        }

        virtual public string ProxyName
        {
            get { return PROXY_NAME; }
        }

        public string ProxyUserId
        {
            get { return _proxyUserId; }
            set { _proxyUserId = value; }
        }

        public TcpClient TcpClient
        {
            get { return _tcpClient; }
            set { _tcpClient = value; }
        }


        public TcpClient CreateConnection(string destinationHost, ushort destinationPort)
        {
            if (String.IsNullOrEmpty(destinationHost))
                throw new ArgumentNullException("destinationHost");

            if (destinationPort <= 0 || destinationPort > 65535)
                throw new ArgumentOutOfRangeException("destinationPort", "port must be greater than zero and less than 65535");

            try
            {
                // if we have no connection, create one
                if (_tcpClient == null)
                {
                    if (String.IsNullOrEmpty(_proxyHost))
                        throw new ProxyException("ProxyHost property must contain a value.");

                    if (_proxyPort <= 0 || _proxyPort > 65535)
                        throw new ProxyException("ProxyPort value must be greater than zero and less than 65535");

                    //  create new tcp client object to the proxy server
                    _tcpClient = new TcpClient();

                    // attempt to open the connection
                    _tcpClient.Connect(_proxyHost, _proxyPort);
                }

                //  send connection command to proxy host for the specified destination host and port
                SendCommand(_tcpClient.GetStream(), SOCKS4_CMD_CONNECT, destinationHost, destinationPort, _proxyUserId);

                // return the open proxied tcp client object to the caller for normal use
                return _tcpClient;
            }
            catch (Exception ex)
            {
                throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "Connection to proxy host {0} on port {1} failed.", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient)), ex);
            }
        }


        internal virtual void SendCommand(NetworkStream proxy, byte command, string destinationHost, int destinationPort, string userId)
        {

            if (userId == null)
                userId = "";

            byte[] destIp = GetIPAddressBytes(destinationHost);
            byte[] destPort = GetDestinationPortBytes(destinationPort);
            byte[] userIdBytes = ASCIIEncoding.ASCII.GetBytes(userId);
            byte[] request = new byte[9 + userIdBytes.Length];

            //  set the bits on the request byte array
            request[0] = SOCKS4_VERSION_NUMBER;
            request[1] = command;
            destPort.CopyTo(request, 2);
            destIp.CopyTo(request, 4);
            userIdBytes.CopyTo(request, 8);
            request[8 + userIdBytes.Length] = 0x00;  // null (byte with all zeros) terminator for userId

            // send the connect request
            proxy.Write(request, 0, request.Length);



            byte[] response = new byte[8];

            proxy.ReadToEnd(response, 8);

            if (response[1] != SOCKS4_CMD_REPLY_REQUEST_GRANTED)
                HandleProxyCommandError(response, destinationHost, destinationPort);
        }


        internal byte[] GetIPAddressBytes(string destinationHost)
        {
            IPAddress ipAddr = null;

            if (!IPAddress.TryParse(destinationHost, out ipAddr))
            {
                try
                {
                    ipAddr = Dns.GetHostEntry(destinationHost).AddressList[0];
                }
                catch (Exception ex)
                {
                    throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "A error occurred while attempting to DNS resolve the host name {0}.", destinationHost), ex);
                }
            }

            // return address bytes
            return ipAddr.GetAddressBytes();
        }


        internal byte[] GetDestinationPortBytes(int value)
        {
            byte[] array = new byte[2];
            array[0] = Convert.ToByte(value / 256);
            array[1] = Convert.ToByte(value % 256);
            return array;
        }
        private static void GetDestinationPortBytes(ushort value, Span<byte> buffer)
        {
            BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        }

        internal void HandleProxyCommandError(byte[] response, string destinationHost, int destinationPort)
        {

            if (response == null)
                throw new ArgumentNullException("response");

            //  extract the reply code
            byte replyCode = response[1];

            //  extract the ip v4 address (4 bytes)
            byte[] ipBytes = new byte[4];
            for (int i = 0; i < 4; i++)
                ipBytes[i] = response[i + 4];

            //  convert the ip address to an IPAddress object
            IPAddress ipAddr = new IPAddress(ipBytes);

            //  extract the port number big endian (2 bytes)
            byte[] portBytes = new byte[2];
            portBytes[0] = response[3];
            portBytes[1] = response[2];
            Int16 port = BitConverter.ToInt16(portBytes, 0);

            // translate the reply code error number to human readable text
            string proxyErrorText;
            switch (replyCode)
            {
                case SOCKS4_CMD_REPLY_REQUEST_REJECTED_OR_FAILED:
                    proxyErrorText = "connection request was rejected or failed";
                    break;
                case SOCKS4_CMD_REPLY_REQUEST_REJECTED_CANNOT_CONNECT_TO_IDENTD:
                    proxyErrorText = "connection request was rejected because SOCKS destination cannot connect to identd on the client";
                    break;
                case SOCKS4_CMD_REPLY_REQUEST_REJECTED_DIFFERENT_IDENTD:
                    proxyErrorText = "connection request rejected because the client program and identd report different user-ids";
                    break;
                default:
                    proxyErrorText = String.Format(CultureInfo.InvariantCulture, "proxy client received an unknown reply with the code value '{0}' from the proxy destination", replyCode.ToString(CultureInfo.InvariantCulture));
                    break;
            }

            //  build the exeception message string
            string exceptionMsg = String.Format(CultureInfo.InvariantCulture, "The {0} concerning destination host {1} port number {2}.  The destination reported the host as {3} port {4}.", proxyErrorText, destinationHost, destinationPort, ipAddr.ToString(), port.ToString(CultureInfo.InvariantCulture));

            //  throw a new application exception 
            throw new ProxyException(exceptionMsg);
        }


        public async ValueTask<TcpClient> CreateConnectionAsync(string destinationHost, ushort destinationPort, CancellationToken cancellation = default)
        {
            if (String.IsNullOrEmpty(destinationHost))
                throw new ArgumentNullException("destinationHost");

            if (destinationPort <= 0 || destinationPort > 65535)
                throw new ArgumentOutOfRangeException("destinationPort", "port must be greater than zero and less than 65535");

            try
            {
                // if we have no connection, create one
                if (_tcpClient == null)
                {
                    if (String.IsNullOrEmpty(_proxyHost))
                        throw new ProxyException("ProxyHost property must contain a value.");

                    if (_proxyPort <= 0 || _proxyPort > 65535)
                        throw new ProxyException("ProxyPort value must be greater than zero and less than 65535");

                    //  create new tcp client object to the proxy server
                    _tcpClient = new TcpClient();

                    // attempt to open the connection
                    await _tcpClient.ConnectAsync(_proxyHost, _proxyPort, cancellation);
                }

                //  send connection command to proxy host for the specified destination host and port
                await SendCommandAsync(_tcpClient.GetStream(), SOCKS4_CMD_CONNECT, destinationHost, destinationPort, _proxyUserId, cancellation);

                // return the open proxied tcp client object to the caller for normal use
                return _tcpClient;
            }
            catch (Exception ex)
            {
                throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "Connection to proxy host {0} on port {1} failed.", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient)), ex);
            }
        }
        internal async ValueTask<byte[]> GetIPAddressBytesAsync(string destinationHost, CancellationToken token)
        {
            IPAddress ipAddr = null;

            if (!IPAddress.TryParse(destinationHost, out ipAddr))
            {
                try
                {
                    ipAddr = (await Dns.GetHostEntryAsync(destinationHost, token)).AddressList[0];
                }
                catch (Exception ex)
                {
                    throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "A error occurred while attempting to DNS resolve the host name {0}.", destinationHost), ex);
                }
            }
            // return address bytes
            return ipAddr.GetAddressBytes();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteRequest(Span<byte> buffer, byte command, ushort port, byte[] destIp, string userId)
        {
            buffer[0] = SOCKS4_VERSION_NUMBER;
            buffer[1] = command;
            GetDestinationPortBytes(port, buffer);
            buffer = buffer.Slice(2);
            destIp.CopyTo(buffer);
            buffer = buffer.Slice(destIp.Length);
            ASCIIEncoding.ASCII.GetBytes(userId, buffer);
            buffer[userId.Length] = 0x00;
        }
        internal virtual async ValueTask SendCommandAsync(NetworkStream proxy, byte command, string destinationHost, ushort destinationPort, string userId, CancellationToken cancellation)
        {
            proxy.ReadTimeout = 15000;
            if (userId == null)
                userId = "";
            int len = userId.Length + 9;
            Memory<byte> request = MemoryPool<byte>.Shared.Rent(len).Memory;

            byte[] destIp = await GetIPAddressBytesAsync(destinationHost, cancellation);


            WriteRequest(request.Span, command, destinationPort, destIp, userId);

            //byte[] destPort = GetDestinationPortBytes(destinationPort);
            //byte[] userIdBytes = ASCIIEncoding.ASCII.GetBytes(userId);
            //byte[] request = new byte[9 + userIdBytes.Length];

            ////  set the bits on the request byte array
            //request[0] = SOCKS4_VERSION_NUMBER;
            //request[1] = command;
            //destPort.CopyTo(request, 2);
            //destIp.CopyTo(request, 4);
            //userIdBytes.CopyTo(request, 8);
            //request[8 + userIdBytes.Length] = 0x00;  // null (byte with all zeros) terminator for userId




            // send the connect request
            await proxy.WriteAsync(request.Slice(0, len), cancellation);



            byte[] response = new byte[8];

            await proxy.ReadToEndAsync(response, 8, cancellation);

            if (response[1] != SOCKS4_CMD_REPLY_REQUEST_GRANTED)
                HandleProxyCommandError(response, destinationHost, destinationPort);
        }

    }

}
