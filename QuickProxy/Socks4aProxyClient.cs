using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace QuickProxy
{
    /// <summary>
    /// Socks4a connection proxy class.  This class implements the Socks4a standard proxy protocol
    /// which is an extension of the Socks4 protocol 
    /// </summary>
    /// <remarks>
    /// In Socks version 4A if the client cannot resolve the destination host's domain name 
    /// to find its IP address the server will attempt to resolve it.  
    /// </remarks>
    public class Socks4aProxyClient : Socks4ProxyClient
    {
        private const string PROXY_NAME = "SOCKS4a";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Socks4aProxyClient()
            : base()
        { }

        /// <summary>
        /// Creates a Socks4 proxy client object using the supplied TcpClient object connection.
        /// </summary>
        /// <param name="tcpClient">An open TcpClient object with an established connection.</param>
        public Socks4aProxyClient(TcpClient tcpClient)
            : base(tcpClient)
        { }

        /// <summary>
        /// Create a Socks4a proxy client object.  The default proxy port 1080 is used.
        /// </summary>
        /// <param name="proxyHost">Host name or IP address of the proxy server.</param>
        /// <param name="proxyUserId">Proxy user identification information for an IDENTD server.</param>
        public Socks4aProxyClient(string proxyHost, string proxyUserId)
            : base(proxyHost, proxyUserId)
        { }

        /// <summary>
        /// Create a Socks4a proxy client object.
        /// </summary>
        /// <param name="proxyHost">Host name or IP address of the proxy server.</param>
        /// <param name="proxyPort">Port used to connect to proxy server.</param>
        /// <param name="proxyUserId">Proxy user identification information.</param>
        public Socks4aProxyClient(string proxyHost, ushort proxyPort, string proxyUserId)
            : base(proxyHost, proxyPort, proxyUserId)
        { }

        /// <summary>
        /// Create a Socks4 proxy client object.  The default proxy port 1080 is used.
        /// </summary>
        /// <param name="proxyHost">Host name or IP address of the proxy server.</param>
        public Socks4aProxyClient(string proxyHost) : base(proxyHost)
        { }

        /// <summary>
        /// Create a Socks4a proxy client object.
        /// </summary>
        /// <param name="proxyHost">Host name or IP address of the proxy server.</param>
        /// <param name="proxyPort">Port used to connect to proxy server.</param>
        public Socks4aProxyClient(string proxyHost, ushort proxyPort)
            : base(proxyHost, proxyPort)
        { }

        /// <summary>
        /// Gets String representing the name of the proxy. 
        /// </summary>
        /// <remarks>This property will always return the value 'SOCKS4a'</remarks>
        public override string ProxyName
        {
            get { return PROXY_NAME; }
        }
        internal override async ValueTask SendCommandAsync(NetworkStream proxy, byte command, string destinationHost, ushort destinationPort, string userId, CancellationToken cancellation)
        {

            if (userId == null)
                userId = "";

            byte[] destIp = { 0, 0, 0, 1 };  // build the invalid ip address as specified in the 4a protocol
            byte[] destPort = GetDestinationPortBytes(destinationPort);
            byte[] userIdBytes = ASCIIEncoding.ASCII.GetBytes(userId);
            byte[] hostBytes = ASCIIEncoding.ASCII.GetBytes(destinationHost);
            byte[] request = new byte[10 + userIdBytes.Length + hostBytes.Length];

            //  set the bits on the request byte array
            request[0] = SOCKS4_VERSION_NUMBER;
            request[1] = command;
            destPort.CopyTo(request, 2);
            destIp.CopyTo(request, 4);
            userIdBytes.CopyTo(request, 8);  // copy the userid to the request byte array
            request[8 + userIdBytes.Length] = 0x00;  // null (byte with all zeros) terminator for userId
            hostBytes.CopyTo(request, 9 + userIdBytes.Length);  // copy the host name to the request byte array
            request[9 + userIdBytes.Length + hostBytes.Length] = 0x00;  // null (byte with all zeros) terminator for userId

            // send the connect request
            await proxy.WriteAsync(request, 0, request.Length, cancellation);


            byte[] response = new byte[8];

            // read the resonse from the network stream
            await proxy.ReadToEndAsync(response, 8, cancellation);

            //  evaluate the reply code for an error condition
            if (response[1] != SOCKS4_CMD_REPLY_REQUEST_GRANTED)
                HandleProxyCommandError(response, destinationHost, destinationPort);
        }

        internal override void SendCommand(NetworkStream proxy, byte command, string destinationHost, int destinationPort, string userId)
        {

            if (userId == null)
                userId = "";

            byte[] destIp = { 0, 0, 0, 1 };  // build the invalid ip address as specified in the 4a protocol
            byte[] destPort = GetDestinationPortBytes(destinationPort);
            byte[] userIdBytes = ASCIIEncoding.ASCII.GetBytes(userId);
            byte[] hostBytes = ASCIIEncoding.ASCII.GetBytes(destinationHost);
            byte[] request = new byte[10 + userIdBytes.Length + hostBytes.Length];

            //  set the bits on the request byte array
            request[0] = SOCKS4_VERSION_NUMBER;
            request[1] = command;
            destPort.CopyTo(request, 2);
            destIp.CopyTo(request, 4);
            userIdBytes.CopyTo(request, 8);  // copy the userid to the request byte array
            request[8 + userIdBytes.Length] = 0x00;  // null (byte with all zeros) terminator for userId
            hostBytes.CopyTo(request, 9 + userIdBytes.Length);  // copy the host name to the request byte array
            request[9 + userIdBytes.Length + hostBytes.Length] = 0x00;  // null (byte with all zeros) terminator for userId

            // send the connect request
            proxy.Write(request, 0, request.Length);


            byte[] response = new byte[8];

            // read the resonse from the network stream
            proxy.Read(response, 0, 8);

            //  evaluate the reply code for an error condition
            if (response[1] != SOCKS4_CMD_REPLY_REQUEST_GRANTED)
                HandleProxyCommandError(response, destinationHost, destinationPort);
        }



    }
}
