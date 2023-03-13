using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace QuickProxy
{

    public class Socks5ProxyClient : IProxyClient
    {
        private string _proxyHost;
        private ushort _proxyPort;
        private string _proxyUserName;
        private string _proxyPassword;
        private SocksAuthentication _proxyAuthMethod;
        private TcpClient _tcpClient;

        private const string PROXY_NAME = "SOCKS5";
        private const int SOCKS5_DEFAULT_PORT = 1080;

        private const byte SOCKS5_VERSION_NUMBER = 5;
        private const byte SOCKS5_RESERVED = 0x00;
        private const byte SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED = 0x00;
        private const byte SOCKS5_AUTH_METHOD_GSSAPI = 0x01;
        private const byte SOCKS5_AUTH_METHOD_USERNAME_PASSWORD = 0x02;
        private const byte SOCKS5_AUTH_METHOD_IANA_ASSIGNED_RANGE_BEGIN = 0x03;
        private const byte SOCKS5_AUTH_METHOD_IANA_ASSIGNED_RANGE_END = 0x7f;
        private const byte SOCKS5_AUTH_METHOD_RESERVED_RANGE_BEGIN = 0x80;
        private const byte SOCKS5_AUTH_METHOD_RESERVED_RANGE_END = 0xfe;
        private const byte SOCKS5_AUTH_METHOD_REPLY_NO_ACCEPTABLE_METHODS = 0xff;
        private const byte SOCKS5_CMD_CONNECT = 0x01;
        private const byte SOCKS5_CMD_BIND = 0x02;
        private const byte SOCKS5_CMD_UDP_ASSOCIATE = 0x03;
        private const byte SOCKS5_CMD_REPLY_SUCCEEDED = 0x00;
        private const byte SOCKS5_CMD_REPLY_GENERAL_SOCKS_SERVER_FAILURE = 0x01;
        private const byte SOCKS5_CMD_REPLY_CONNECTION_NOT_ALLOWED_BY_RULESET = 0x02;
        private const byte SOCKS5_CMD_REPLY_NETWORK_UNREACHABLE = 0x03;
        private const byte SOCKS5_CMD_REPLY_HOST_UNREACHABLE = 0x04;
        private const byte SOCKS5_CMD_REPLY_CONNECTION_REFUSED = 0x05;
        private const byte SOCKS5_CMD_REPLY_TTL_EXPIRED = 0x06;
        private const byte SOCKS5_CMD_REPLY_COMMAND_NOT_SUPPORTED = 0x07;
        private const byte SOCKS5_CMD_REPLY_ADDRESS_TYPE_NOT_SUPPORTED = 0x08;
        private const byte SOCKS5_ADDRTYPE_IPV4 = 0x01;
        private const byte SOCKS5_ADDRTYPE_DOMAIN_NAME = 0x03;
        private const byte SOCKS5_ADDRTYPE_IPV6 = 0x04;

        private enum SocksAuthentication
        {

            None,

            UsernamePassword
        }

        public Socks5ProxyClient() { }

        public Socks5ProxyClient(TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");

            _tcpClient = tcpClient;
        }


        public Socks5ProxyClient(string proxyHost)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            _proxyHost = proxyHost;
            _proxyPort = SOCKS5_DEFAULT_PORT;
        }


        public Socks5ProxyClient(string proxyHost, ushort proxyPort)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            if (proxyPort <= 0 || proxyPort > 65535)
                throw new ArgumentOutOfRangeException("proxyPort", "port must be greater than zero and less than 65535");

            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
        }


        public Socks5ProxyClient(string proxyHost, string proxyUserName, string proxyPassword)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            if (proxyUserName == null)
                throw new ArgumentNullException("proxyUserName");

            if (proxyPassword == null)
                throw new ArgumentNullException("proxyPassword");

            _proxyHost = proxyHost;
            _proxyPort = SOCKS5_DEFAULT_PORT;
            _proxyUserName = proxyUserName;
            _proxyPassword = proxyPassword;
        }

        public Socks5ProxyClient(string proxyHost, ushort proxyPort, string proxyUserName, string proxyPassword)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            if (proxyPort <= 0 || proxyPort > 65535)
                throw new ArgumentOutOfRangeException("proxyPort", "port must be greater than zero and less than 65535");

            if (proxyUserName == null)
                throw new ArgumentNullException("proxyUserName");

            if (proxyPassword == null)
                throw new ArgumentNullException("proxyPassword");

            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
            _proxyUserName = proxyUserName;
            _proxyPassword = proxyPassword;
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

        public string ProxyName
        {
            get { return PROXY_NAME; }
        }

        public string ProxyUserName
        {
            get { return _proxyUserName; }
            set { _proxyUserName = value; }
        }

        public string ProxyPassword
        {
            get { return _proxyPassword; }
            set { _proxyPassword = value; }
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

                //  determine which authentication method the client would like to use
                DetermineClientAuthMethod();

                // negotiate which authentication methods are supported / accepted by the server
                NegotiateServerAuthMethod();

                // send a connect command to the proxy server for destination host and port
                SendCommand(SOCKS5_CMD_CONNECT, destinationHost, destinationPort);

                // return the open proxied tcp client object to the caller for normal use
                return _tcpClient;
            }
            catch (Exception ex)
            {
                throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "Connection to proxy host {0} on port {1} failed.", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient)), ex);
            }
        }


        private void DetermineClientAuthMethod()
        {
            //  set the authentication itemType used based on values inputed by the user
            if (_proxyUserName != null && _proxyPassword != null)
                _proxyAuthMethod = SocksAuthentication.UsernamePassword;
            else
                _proxyAuthMethod = SocksAuthentication.None;
        }

        private void NegotiateServerAuthMethod()
        {

            NetworkStream stream = _tcpClient.GetStream();


            var haveUserPass = !String.IsNullOrEmpty(_proxyUserName) &&
                               !String.IsNullOrEmpty(_proxyPassword);

            //var authRequest = new List<byte>();
            Span<byte> authRequest = default;
            int len = 0;
            if (haveUserPass)
            {
                authRequest = ArrayPool<byte>.Shared.Rent(5);
                authRequest[0] = SOCKS5_VERSION_NUMBER;
                authRequest[1] = 2;
                authRequest[2] = SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED;
                authRequest[3] = SOCKS5_AUTH_METHOD_USERNAME_PASSWORD;
                len = 4;
            }
            else
            {
                authRequest = ArrayPool<byte>.Shared.Rent(5);
                authRequest[0] = SOCKS5_VERSION_NUMBER;
                authRequest[1] = 1;
                authRequest[2] = SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED;
                len = 3;
            }

            //  send the request to the server specifying authentication types supported by the client.
            stream.Write(authRequest.Slice(0, len));

            Span<byte> response = stackalloc byte[2];
            stream.Read(response);

            byte acceptedAuthMethod = response[1];

            // if the server does not accept any of our supported authenication methods then throw an error
            if (acceptedAuthMethod == SOCKS5_AUTH_METHOD_REPLY_NO_ACCEPTABLE_METHODS)
            {
                _tcpClient.Close();
                throw new ProxyException("The proxy destination does not accept the supported proxy client authentication methods.");
            }

            // if the server accepts a username and password authentication and none is provided by the user then throw an error
            if (acceptedAuthMethod == SOCKS5_AUTH_METHOD_USERNAME_PASSWORD && _proxyAuthMethod == SocksAuthentication.None)
            {
                _tcpClient.Close();
                throw new ProxyException("The proxy destination requires a username and password for authentication.");
            }

            if (acceptedAuthMethod == SOCKS5_AUTH_METHOD_USERNAME_PASSWORD)
            {

                byte[] credentials = new byte[_proxyUserName.Length + _proxyPassword.Length + 3];
                credentials[0] = 1;
                credentials[1] = (byte)_proxyUserName.Length;
                Array.Copy(ASCIIEncoding.ASCII.GetBytes(_proxyUserName), 0, credentials, 2, _proxyUserName.Length);
                credentials[_proxyUserName.Length + 2] = (byte)_proxyPassword.Length;
                Array.Copy(ASCIIEncoding.ASCII.GetBytes(_proxyPassword), 0, credentials, _proxyUserName.Length + 3, _proxyPassword.Length);


                stream.Write(credentials, 0, credentials.Length);
                byte[] crResponse = new byte[2];
                stream.Read(crResponse, 0, crResponse.Length);

                if (crResponse[1] != 0)
                {
                    _tcpClient.Close();
                    throw new ProxyException("Proxy authentification failure!");
                }
            }
        }

        private byte GetDestAddressType(string host)
        {
            IPAddress ipAddr = null;

            bool result = IPAddress.TryParse(host, out ipAddr);

            if (!result)
                return SOCKS5_ADDRTYPE_DOMAIN_NAME;

            switch (ipAddr.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    return SOCKS5_ADDRTYPE_IPV4;
                case AddressFamily.InterNetworkV6:
                    return SOCKS5_ADDRTYPE_IPV6;
                default:
                    throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "The host addess {0} of type '{1}' is not a supported address type.  The supported types are InterNetwork and InterNetworkV6.", host, Enum.GetName(typeof(AddressFamily), ipAddr.AddressFamily)));
            }

        }

        private byte[] GetDestAddressBytes(byte addressType, string host)
        {
            switch (addressType)
            {
                case SOCKS5_ADDRTYPE_IPV4:
                case SOCKS5_ADDRTYPE_IPV6:
                    return IPAddress.Parse(host).GetAddressBytes();
                case SOCKS5_ADDRTYPE_DOMAIN_NAME:
                    //  create a byte array to hold the host name bytes plus one byte to store the length
                    byte[] bytes = new byte[host.Length + 1];
                    //  if the address field contains a fully-qualified domain name.  The first
                    //  octet of the address field contains the number of octets of name that
                    //  follow, there is no terminating NUL octet.
                    bytes[0] = Convert.ToByte(host.Length);
                    Encoding.ASCII.GetBytes(host).CopyTo(bytes, 1);
                    return bytes;
                default:
                    return null;
            }
        }

        private byte[] GetDestPortBytes(int value)
        {
            byte[] array = new byte[2];
            array[0] = Convert.ToByte(value / 256);
            array[1] = Convert.ToByte(value % 256);
            return array;
        }

        private void SendCommand(byte command, string destinationHost, ushort destinationPort)
        {
            NetworkStream stream = _tcpClient.GetStream();

            byte addressType = GetDestAddressType(destinationHost);
            byte[] destAddr = GetDestAddressBytes(addressType, destinationHost);
            byte[] destPort = GetDestPortBytes(destinationPort);


            byte[] request = new byte[4 + destAddr.Length + 2];
            request[0] = SOCKS5_VERSION_NUMBER;
            request[1] = command;
            request[2] = SOCKS5_RESERVED;
            request[3] = addressType;
            destAddr.CopyTo(request, 4);
            destPort.CopyTo(request, 4 + destAddr.Length);

            // send connect request.
            stream.Write(request, 0, request.Length);



            byte[] response = new byte[255];

            // read proxy server response
            var responseSize = stream.Read(response, 0, response.Length);

            byte replyCode = response[1];

            //  evaluate the reply code for an error condition
            if (responseSize < 2 || replyCode != SOCKS5_CMD_REPLY_SUCCEEDED)
                HandleProxyCommandError(response, destinationHost, destinationPort);
        }

        private void HandleProxyCommandError(Span<byte> response, string destinationHost, int destinationPort)
        {
            string proxyErrorText;
            byte replyCode = response[1];
            byte addrType = response[3];
            string addr = "";
            Int16 port = 0;

            switch (addrType)
            {
                case SOCKS5_ADDRTYPE_DOMAIN_NAME:
                    int addrLen = Convert.ToInt32(response[4]);
                    byte[] addrBytes = new byte[addrLen];
                    for (int i = 0; i < addrLen; i++)
                        addrBytes[i] = response[i + 5];
                    addr = System.Text.ASCIIEncoding.ASCII.GetString(addrBytes);
                    byte[] portBytesDomain = new byte[2];
                    portBytesDomain[0] = response[6 + addrLen];
                    portBytesDomain[1] = response[5 + addrLen];
                    port = BitConverter.ToInt16(portBytesDomain, 0);
                    break;

                case SOCKS5_ADDRTYPE_IPV4:
                    byte[] ipv4Bytes = new byte[4];
                    for (int i = 0; i < 4; i++)
                        ipv4Bytes[i] = response[i + 4];
                    IPAddress ipv4 = new IPAddress(ipv4Bytes);
                    addr = ipv4.ToString();
                    byte[] portBytesIpv4 = new byte[2];
                    portBytesIpv4[0] = response[9];
                    portBytesIpv4[1] = response[8];
                    port = BitConverter.ToInt16(portBytesIpv4, 0);
                    break;

                case SOCKS5_ADDRTYPE_IPV6:
                    byte[] ipv6Bytes = new byte[16];
                    for (int i = 0; i < 16; i++)
                        ipv6Bytes[i] = response[i + 4];
                    IPAddress ipv6 = new IPAddress(ipv6Bytes);
                    addr = ipv6.ToString();
                    byte[] portBytesIpv6 = new byte[2];
                    portBytesIpv6[0] = response[21];
                    portBytesIpv6[1] = response[20];
                    port = BitConverter.ToInt16(portBytesIpv6, 0);
                    break;
            }


            switch (replyCode)
            {
                case SOCKS5_CMD_REPLY_GENERAL_SOCKS_SERVER_FAILURE:
                    proxyErrorText = "a general socks destination failure occurred";
                    break;
                case SOCKS5_CMD_REPLY_CONNECTION_NOT_ALLOWED_BY_RULESET:
                    proxyErrorText = "the connection is not allowed by proxy destination rule set";
                    break;
                case SOCKS5_CMD_REPLY_NETWORK_UNREACHABLE:
                    proxyErrorText = "the network was unreachable";
                    break;
                case SOCKS5_CMD_REPLY_HOST_UNREACHABLE:
                    proxyErrorText = "the host was unreachable";
                    break;
                case SOCKS5_CMD_REPLY_CONNECTION_REFUSED:
                    proxyErrorText = "the connection was refused by the remote network";
                    break;
                case SOCKS5_CMD_REPLY_TTL_EXPIRED:
                    proxyErrorText = "the time to live (TTL) has expired";
                    break;
                case SOCKS5_CMD_REPLY_COMMAND_NOT_SUPPORTED:
                    proxyErrorText = "the command issued by the proxy client is not supported by the proxy destination";
                    break;
                case SOCKS5_CMD_REPLY_ADDRESS_TYPE_NOT_SUPPORTED:
                    proxyErrorText = "the address type specified is not supported";
                    break;
                default:
                    proxyErrorText = String.Format(CultureInfo.InvariantCulture, "that an unknown reply with the code value '{0}' was received by the destination", replyCode.ToString(CultureInfo.InvariantCulture));
                    break;
            }
            string exceptionMsg = String.Format(CultureInfo.InvariantCulture, "The {0} concerning destination host {1} port number {2}.  The destination reported the host as {3} port {4}.", proxyErrorText, destinationHost, destinationPort, addr, port.ToString(CultureInfo.InvariantCulture));

            throw new ProxyException(exceptionMsg);

        }


        #region Async

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

                //  determine which authentication method the client would like to use
                DetermineClientAuthMethod();

                // negotiate which authentication methods are supported / accepted by the server
                await NegotiateServerAuthMethodAsync(cancellation);

                // send a connect command to the proxy server for destination host and port
                await SendCommandAsync(SOCKS5_CMD_CONNECT, destinationHost, destinationPort, cancellation);

                // return the open proxied tcp client object to the caller for normal use
                return _tcpClient;
            }
            catch (Exception ex)
            {
                throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "Connection to proxy host {0} on port {1} failed.", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient)), ex);
            }
        }
        private async ValueTask NegotiateServerAuthMethodAsync(CancellationToken token)
        {

            NetworkStream stream = _tcpClient.GetStream();


            var haveUserPass = !String.IsNullOrEmpty(_proxyUserName) &&
                               !String.IsNullOrEmpty(_proxyPassword);

            var authRequest = new List<byte>();
            authRequest.Add(SOCKS5_VERSION_NUMBER);
            if (haveUserPass)
            {
                authRequest.Add(2);
                authRequest.Add(SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED);
                authRequest.Add(SOCKS5_AUTH_METHOD_USERNAME_PASSWORD);
            }
            else
            {
                authRequest.Add(1);
                authRequest.Add(SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED);
            }

            //  send the request to the server specifying authentication types supported by the client.
            await stream.WriteAsync(authRequest.ToArray(), 0, authRequest.Count, token);

            byte[] response = new byte[2];
            await stream.ReadToEndAsync(response, response.Length, token);

            byte acceptedAuthMethod = response[1];

            // if the server does not accept any of our supported authenication methods then throw an error
            if (acceptedAuthMethod == SOCKS5_AUTH_METHOD_REPLY_NO_ACCEPTABLE_METHODS)
            {
                _tcpClient.Close();
                throw new ProxyException("The proxy destination does not accept the supported proxy client authentication methods.");
            }

            // if the server accepts a username and password authentication and none is provided by the user then throw an error
            if (acceptedAuthMethod == SOCKS5_AUTH_METHOD_USERNAME_PASSWORD && _proxyAuthMethod == SocksAuthentication.None)
            {
                _tcpClient.Close();
                throw new ProxyException("The proxy destination requires a username and password for authentication.");
            }

            if (acceptedAuthMethod == SOCKS5_AUTH_METHOD_USERNAME_PASSWORD)
            {

                byte[] credentials = new byte[_proxyUserName.Length + _proxyPassword.Length + 3];
                credentials[0] = 1;
                credentials[1] = (byte)_proxyUserName.Length;
                Array.Copy(ASCIIEncoding.ASCII.GetBytes(_proxyUserName), 0, credentials, 2, _proxyUserName.Length);
                credentials[_proxyUserName.Length + 2] = (byte)_proxyPassword.Length;
                Array.Copy(ASCIIEncoding.ASCII.GetBytes(_proxyPassword), 0, credentials, _proxyUserName.Length + 3, _proxyPassword.Length);


                await stream.WriteAsync(credentials, 0, credentials.Length, token);
                byte[] crResponse = new byte[2];
                await stream.ReadToEndAsync(crResponse, crResponse.Length, token);

                if (crResponse[1] != 0)
                {
                    _tcpClient.Close();
                    throw new ProxyException("Proxy authentification failure!");
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CreateRequest(Span<byte> buffer, byte[] destAddr, byte command, byte addressType, byte[] destPort)
        {
            buffer[0] = SOCKS5_VERSION_NUMBER;
            buffer[1] = command;
            buffer[2] = SOCKS5_RESERVED;
            buffer[3] = addressType;
            buffer = buffer.Slice(4);
            destAddr.CopyTo(buffer);
            buffer.Slice(destAddr.Length);
            destPort.CopyTo(buffer);
        }

        private async ValueTask SendCommandAsync(byte command, string destinationHost, ushort destinationPort, CancellationToken token)
        {
            NetworkStream stream = _tcpClient.GetStream();

            byte addressType = GetDestAddressType(destinationHost);
            byte[] destAddr = GetDestAddressBytes(addressType, destinationHost);

            byte[] destPort = GetDestPortBytes(destinationPort);

            int len = 4 + destAddr.Length + 2;
            Memory<byte> request = MemoryPool<byte>.Shared.Rent(len).Memory;

            CreateRequest(request.Span, destAddr, command, addressType, destPort);

            // send connect request.
            await stream.WriteAsync(request.Slice(0, len), token);



            Memory<byte> response = MemoryPool<byte>.Shared.Rent(255).Memory;

            // read proxy server response
            var responseSize = await stream.ReadAsync(response.Slice(0, 255), token);

            byte replyCode = response.Span[1];

            //  evaluate the reply code for an error condition
            if (responseSize < 2 || replyCode != SOCKS5_CMD_REPLY_SUCCEEDED)
                HandleProxyCommandError(response.Span, destinationHost, destinationPort);
        }

        #endregion
    }
}
