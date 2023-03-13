using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Buffers;

namespace QuickProxy
{
    public class HttpProxyClient : IProxyClient
    {
        private const string HTTP_PROXY_CONNECT_CMD = "CONNECT {0}:{1} HTTP/1.0\r\nHost: {0}:{1}\r\n\r\n";
        private string _proxyHost;
        private ushort _proxyPort;
        private HttpResponseCodes _respCode;
        private string _respText;
        private TcpClient _tcpClient;

        private const int HTTP_PROXY_DEFAULT_PORT = 8080;

        private const int WAIT_FOR_DATA_INTERVAL = 50; // 50 ms
        private const int WAIT_FOR_DATA_TIMEOUT = 15000; // 15 seconds
        private const string PROXY_NAME = "HTTP";

        private enum HttpResponseCodes
        {
            None = 0,
            Continue = 100,
            SwitchingProtocols = 101,
            OK = 200,
            Created = 201,
            Accepted = 202,
            NonAuthoritiveInformation = 203,
            NoContent = 204,
            ResetContent = 205,
            PartialContent = 206,
            MultipleChoices = 300,
            MovedPermanetly = 301,
            Found = 302,
            SeeOther = 303,
            NotModified = 304,
            UserProxy = 305,
            TemporaryRedirect = 307,
            BadRequest = 400,
            Unauthorized = 401,
            PaymentRequired = 402,
            Forbidden = 403,
            NotFound = 404,
            MethodNotAllowed = 405,
            NotAcceptable = 406,
            ProxyAuthenticantionRequired = 407,
            RequestTimeout = 408,
            Conflict = 409,
            Gone = 410,
            PreconditionFailed = 411,
            RequestEntityTooLarge = 413,
            RequestURITooLong = 414,
            UnsupportedMediaType = 415,
            RequestedRangeNotSatisfied = 416,
            ExpectationFailed = 417,
            InternalServerError = 500,
            NotImplemented = 501,
            BadGateway = 502,
            ServiceUnavailable = 503,
            GatewayTimeout = 504,
            HTTPVersionNotSupported = 505
        }

        public HttpProxyClient() { }

        public HttpProxyClient(TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");

            _tcpClient = tcpClient;
        }



        public HttpProxyClient(string proxyHost)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            _proxyHost = proxyHost;
            _proxyPort = HTTP_PROXY_DEFAULT_PORT;
        }

        public HttpProxyClient(string proxyHost, ushort proxyPort)
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

        public string ProxyName
        {
            get { return PROXY_NAME; }
        }

        public TcpClient TcpClient
        {
            get { return _tcpClient; }
            set { _tcpClient = value; }
        }

        #region Sync


        public TcpClient CreateConnection(string destinationHost, ushort destinationPort)
        {
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
                SendConnectionCommand(destinationHost, destinationPort);

                // return the open proxied tcp client object to the caller for normal use
                return _tcpClient;
            }
            catch (SocketException ex)
            {
                throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "Connection to proxy host {0} on port {1} failed.", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient)), ex);
            }
        }


        private const string part1 = "CONNECT ";
        private const string part2 = " HTTP/1.0\r\nHost: ";
        private const string part3 = "\r\n\r\n";
        private const byte TwoPoints = (byte)':';

        public static int GetFullSizeConenctionCommand(int hostPort)
        {
            return part1.Length + part2.Length + part3.Length + hostPort + hostPort;
        }

        public static void CreateRequest(Span<byte> buffer, int size, string host, string port)
        {
            Span<byte> hostPort = stackalloc byte[host.Length + 1 + port.Length];
            ushort i = 0;
            ASCIIEncoding.ASCII.GetBytes(host, hostPort);
            hostPort[host.Length] = TwoPoints;
            ASCIIEncoding.ASCII.GetBytes(port, hostPort.Slice(host.Length + 1));
            Span<byte> request = stackalloc byte[size];
            int offset = 0;
            ASCIIEncoding.ASCII.GetBytes(part1, request);
            offset += part1.Length;
            hostPort.CopyTo(request.Slice(offset));
            offset += hostPort.Length;
            ASCIIEncoding.ASCII.GetBytes(part2, request.Slice(offset));
            offset += part2.Length;
            hostPort.CopyTo(request.Slice(offset));
            offset += hostPort.Length;
            ASCIIEncoding.ASCII.GetBytes(part3, request.Slice(offset));
        }

        private void SendConnectionCommand(string host, int port)
        {
            NetworkStream stream = _tcpClient.GetStream();
            stream.ReadTimeout = 15000;
            stream.WriteTimeout = 15000;


            string portStr = port.ToString();

            int size = GetFullSizeConenctionCommand(host.Length + 1 + portStr.Length);

            Span<byte> request = stackalloc byte[size];
            CreateRequest(request, size, host, portStr);

            stream.Write(request);


            Span<byte> response = stackalloc byte[_tcpClient.ReceiveBufferSize];
            StringBuilder sbuilder = new StringBuilder();
            int bytes = 0;
            long total = 0;

            do
            {
                bytes = stream.Read(response);
                total += bytes;
                sbuilder.Append(System.Text.ASCIIEncoding.UTF8.GetString(response));
            } while (stream.DataAvailable);

            ParseResponse(sbuilder.ToString());

            //  evaluate the reply code for an error condition
            if (_respCode != HttpResponseCodes.OK)
                HandleProxyCommandError(host, port);
        }

        private void HandleProxyCommandError(string host, int port)
        {
            string msg;

            switch (_respCode)
            {
                case HttpResponseCodes.None:
                    msg = String.Format(CultureInfo.InvariantCulture, "Proxy destination {0} on port {1} failed to return a recognized HTTP response code.  Server response: {2}", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient), _respText);
                    break;

                case HttpResponseCodes.BadGateway:
                    //HTTP/1.1 502 Proxy Error (The specified Secure Sockets Layer (SSL) port is not allowed. ISA Server is not configured to allow SSL requests from this port. Most Web browsers use port 443 for SSL requests.)
                    msg = String.Format(CultureInfo.InvariantCulture, "Proxy destination {0} on port {1} responded with a 502 code - Bad Gateway.  If you are connecting to a Microsoft ISA destination please refer to knowledge based article Q283284 for more information.  Server response: {2}", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient), _respText);
                    break;

                default:
                    msg = String.Format(CultureInfo.InvariantCulture, "Proxy destination {0} on port {1} responded with a {2} code - {3}", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient), ((int)_respCode).ToString(CultureInfo.InvariantCulture), _respText);
                    break;
            }

            //  throw a new application exception 
            throw new ProxyException(msg);
        }


        private void ParseResponse(string response)
        {
            string[] data = null;

            //  get rid of the LF character if it exists and then split the string on all CR
            data = response.Replace('\n', ' ').Split('\r');

            ParseCodeAndText(data[0]);
        }

        private void ParseCodeAndText(string line)
        {
            int begin = 0;
            int end = 0;
            string val = null;

            if (line.IndexOf("HTTP") == -1)
                throw new ProxyException(String.Format("No HTTP response received from proxy destination.  Server response: {0}.", line));

            begin = line.IndexOf(" ") + 1;
            end = line.IndexOf(" ", begin);

            val = line.Substring(begin, end - begin);
            Int32 code = 0;

            if (!Int32.TryParse(val, out code))
                throw new ProxyException(String.Format("An invalid response code was received from proxy destination.  Server response: {0}.", line));

            _respCode = (HttpResponseCodes)code;
            _respText = line.Substring(end + 1).Trim();
        }


        #endregion
        public async ValueTask<TcpClient> CreateConnectionAsync(string destinationHost, ushort destinationPort, CancellationToken cancellation = default)
        {
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
                await SendConnectionCommandAsync(destinationHost, destinationPort, cancellation);

                // return the open proxied tcp client object to the caller for normal use
                return _tcpClient;
            }
            catch (SocketException ex)
            {
                throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "Connection to proxy host {0} on port {1} failed.", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient)), ex);
            }
        }
        private async ValueTask SendConnectionCommandAsync(string host, ushort port, CancellationToken token)
        {
            NetworkStream stream = _tcpClient.GetStream();
            stream.ReadTimeout = 15000;
            string portStr = port.ToString();
            int fullsize = GetFullSizeConenctionCommand(host.Length + 1 + portStr.Length);


            Memory<byte> buffer = MemoryPool<byte>.Shared.Rent(_tcpClient.ReceiveBufferSize).Memory;

            CreateRequest(buffer.Span, fullsize, host, portStr);

            await stream.WriteAsync(buffer.Slice(0, fullsize), token);



            StringBuilder sbuilder = new StringBuilder();
            int bytes = 0;
            long total = 0;
            do
            {
                bytes = await stream.ReadAsync(buffer, token);
                total += bytes;
                sbuilder.Append(ASCIIEncoding.UTF8.GetString(buffer.Span.Slice(0, bytes)));
            } while (stream.DataAvailable);

            ParseResponse(sbuilder.ToString());

            //  evaluate the reply code for an error condition
            if (_respCode != HttpResponseCodes.OK)
                HandleProxyCommandError(host, port);
        }



    }
}
