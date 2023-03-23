using System.Buffers;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace QuickProxyNet
{
    public class HttpProxyClient : ProxyClient
    {
        const int BufferSize = 4096;

        public HttpProxyClient(string host, int port) : base(host, port)
        {
        }

        public HttpProxyClient(string host, int port, NetworkCredential credentials) : base(host, port, credentials)
        {
        }

        internal static byte[] GetConnectCommand(string host, int port, NetworkCredential proxyCredentials)
        {
            var builder = new StringBuilder();

            builder.AppendFormat(CultureInfo.InvariantCulture, "CONNECT {0}:{1} HTTP/1.1\r\n", host, port);
            builder.AppendFormat(CultureInfo.InvariantCulture, "Host: {0}:{1}\r\n", host, port);
            if (proxyCredentials != null)
            {
                var token = Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", proxyCredentials.UserName, proxyCredentials.Password));
                var base64 = Convert.ToBase64String(token);
                builder.AppendFormat(CultureInfo.InvariantCulture, "Proxy-Authorization: Basic {0}\r\n", base64);
            }
            builder.Append("\r\n");

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        internal static bool TryConsumeHeaders(StringBuilder builder, byte[] buffer, ref int index, int count, ref bool newLine)
        {
            int endIndex = index + count;
            int startIndex = index;
            var endOfHeaders = false;

            while (index < endIndex && !endOfHeaders)
            {
                switch ((char)buffer[index])
                {
                    case '\r':
                        break;
                    case '\n':
                        endOfHeaders = newLine;
                        newLine = true;
                        break;
                    default:
                        newLine = false;
                        break;
                }

                index++;
            }

            var block = Encoding.UTF8.GetString(buffer, startIndex, index - startIndex);
            builder.Append(block);

            return endOfHeaders;
        }

        internal static void ValidateHttpResponse(StringBuilder builder, string host, int port)
        {
            int index = 0;

            while (builder[index] != '\n')
                index++;

            if (index > 0 && builder[index - 1] == '\r')
                index--;

            // trim everything beyond the "HTTP/1.1 200 ..." part of the response
            builder.Length = index;

            var response = builder.ToString();

            if (response.Length >= 15 && response.StartsWith("HTTP/1.", StringComparison.OrdinalIgnoreCase) &&
                (response[7] == '1' || response[7] == '0') && response[8] == ' ' &&
                response[9] == '2' && response[10] == '0' && response[11] == '0' &&
                response[12] == ' ')
            {
                return;
            }

            throw new ProxyProtocolException(string.Format(CultureInfo.InvariantCulture, "Failed to connect to {0}:{1}: {2}", host, port, response));
        }

        public override Stream Connect(string host, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateArguments(host, port);

            cancellationToken.ThrowIfCancellationRequested();

            var command = GetConnectCommand(host, port, ProxyCredentials);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ProxyHost, ProxyPort);
           // var socket = SocketUtils.Connect(ProxyHost, ProxyPort, LocalEndPoint, cancellationToken);

            try
            {
                socket.Send(command);

                var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
                var builder = new StringBuilder();

                try
                {
                    var newline = false;

                    // read until we consume the end of the headers (it's ok if we read some of the content)
                    do
                    {
                        int nread = socket.Receive(buffer, 0, BufferSize, SocketFlags.None);
                        int index = 0;

                        if (TryConsumeHeaders(builder, buffer, ref index, nread, ref newline))
                            break;
                    } while (true);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }

                ValidateHttpResponse(builder, host, port);
                return new NetworkStream(socket, true);
            }
            catch
            {
                if (socket.Connected)
                    socket.Disconnect(false);
                socket.Dispose();
                throw;
            }
        }

        public override async Task<Stream> ConnectAsync(string host, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateArguments(host, port);

            cancellationToken.ThrowIfCancellationRequested();

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
          await  socket.ConnectAsync(ProxyHost, ProxyPort, cancellationToken);
          //  var socket = await SocketUtils.ConnectAsync(ProxyHost, ProxyPort, LocalEndPoint, cancellationToken);
            var command = GetConnectCommand(host, port, ProxyCredentials);
            int index;

            try
            {
                await socket.SendAsync(command.AsMemory(), SocketFlags.None, cancellationToken);

                var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
                var builder = new StringBuilder();

                try
                {
                    var newline = false;

                    // read until we consume the end of the headers (it's ok if we read some of the content)
                    do
                    {
                        int nread = await socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
                        index = 0;

                        if (TryConsumeHeaders(builder, buffer, ref index, nread, ref newline))
                            break;
                    } while (true);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }

                ValidateHttpResponse(builder, host, port);
                return new NetworkStream(socket, true);
            }
            catch
            {
                if (socket.Connected)
                    socket.Disconnect(false);
                socket.Dispose();
                throw;
            }
        }
    }
}
