using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace QuickProxyNet
{
    public class Socks4Client : SocksClient
    {
        static readonly byte[] InvalidIPAddress = { 0, 0, 0, 1 };

        public Socks4Client(string host, int port) : base(4, host, port)
        {
        }

        public Socks4Client(string host, int port, NetworkCredential credentials) : base(4, host, port, credentials)
        {
        }

        protected bool IsSocks4a
        {
            get; set;
        }

        enum Socks4Command : byte
        {
            Connect = 0x01,
            Bind = 0x02,
        }

        enum Socks4Reply : byte
        {
            RequestGranted = 0x5a,
            RequestRejected = 0x5b,
            RequestFailedNoIdentd = 0x5c,
            RequestFailedWrongId = 0x5d
        }

        static string GetFailureReason(byte reply)
        {
            switch ((Socks4Reply)reply)
            {
                case Socks4Reply.RequestRejected: return "Request rejected or failed.";
                case Socks4Reply.RequestFailedNoIdentd: return "Request failed; unable to contact client machine's identd service.";
                case Socks4Reply.RequestFailedWrongId: return "Request failed; client ID does not match specified username.";
                default: return "Unknown error.";
            }
        }

        static IPAddress Resolve(string host, IPAddress[] ipAddresses)
        {
            for (int i = 0; i < ipAddresses.Length; i++)
            {
                if (ipAddresses[i].AddressFamily == AddressFamily.InterNetwork)
                    return ipAddresses[i];
            }

            throw new ArgumentException($"Could not resolve a suitable IPv4 address for '{host}'.", nameof(host));
        }

        static IPAddress Resolve(string host, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var ipAddresses = Dns.GetHostAddresses(host);

            return Resolve(host, ipAddresses);
        }

        static async ValueTask<IPAddress> ResolveAsync(string host, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var ipAddresses = await Dns.GetHostAddressesAsync(host);

            return Resolve(host, ipAddresses);
        }

        byte[] GetConnectCommand(byte[] domain, byte[] addr, int port)
        {
            // +----+-----+----------+----------+----------+-------+--------------+-------+
            // |VER | CMD | DST.PORT | DST.ADDR |  USERID  | NULL  |  DST.DOMAIN  | NULL  |
            // +----+-----+----------+----------+----------+-------+--------------+-------+
            // | 1  |  1  |    2     |    4     | VARIABLE | X'00' |   VARIABLE   | X'00' |
            // +----+-----+----------+----------+----------+-------+--------------+-------+
            var user = ProxyCredentials != null ? Encoding.UTF8.GetBytes(ProxyCredentials.UserName) : new byte[0];
            int bufferSize = 9 + user.Length + (domain != null ? domain.Length + 1 : 0);
            var buffer = new byte[bufferSize];
            int n = 0;

            buffer[n++] = (byte)SocksVersion;
            buffer[n++] = (byte)Socks4Command.Connect;
            buffer[n++] = (byte)(port >> 8);
            buffer[n++] = (byte)port;
            Buffer.BlockCopy(addr, 0, buffer, n, 4);
            n += 4;
            Buffer.BlockCopy(user, 0, buffer, n, user.Length);
            n += user.Length;
            buffer[n++] = 0x00;

            if (domain != null)
            {
                Buffer.BlockCopy(domain, 0, buffer, n, domain.Length);
                n += domain.Length;
                buffer[n++] = 0x00;
            }

            return buffer;
        }

        public override Stream Connect(string host, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] addr, domain = null;

            ValidateArguments(host, port);

            if (!IPAddress.TryParse(host, out var ip))
            {
                if (IsSocks4a)
                {
                    domain = Encoding.UTF8.GetBytes(host);
                    addr = InvalidIPAddress;
                }
                else
                {
                    ip = Resolve(host, cancellationToken);
                    addr = ip.GetAddressBytes();
                }
            }
            else
            {
                if (ip.AddressFamily != AddressFamily.InterNetwork)
                    throw new ArgumentException("The specified host address must be IPv4.", nameof(host));

                addr = ip.GetAddressBytes();
            }

            cancellationToken.ThrowIfCancellationRequested();
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(ProxyHost, ProxyPort);

            try
            {
                var buffer = GetConnectCommand(domain, addr, port);

                socket.Send(buffer);
                // +-----+-----+----------+----------+
                // | VER | REP | BND.PORT | BND.ADDR |
                // +-----+-----+----------+----------+
                // |  1  |  1  |    2     |    4     |
                // +-----+-----+----------+----------+
                int nread, n = 0;

                do
                {
                    if ((nread = socket.Receive(buffer, 0 + n, 8 - n, SocketFlags.None)) <= 0)
                        throw new EndOfStreamException();
                    n += nread;
                } while (n < 8);

                if (buffer[1] != (byte)Socks4Reply.RequestGranted)
                    throw new ProxyProtocolException(string.Format(CultureInfo.InvariantCulture, "Failed to connect to {0}:{1}: {2}", host, port, GetFailureReason(buffer[1])));

                // TODO: do we care about BND.ADDR and BND.PORT?

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
            byte[] addr, domain = null;

            ValidateArguments(host, port);

            if (!IPAddress.TryParse(host, out var ip))
            {
                if (IsSocks4a)
                {
                    domain = Encoding.UTF8.GetBytes(host);
                    addr = InvalidIPAddress;
                }
                else
                {
                    ip = await ResolveAsync(host, cancellationToken);
                    addr = ip.GetAddressBytes();
                }
            }
            else
            {
                if (ip.AddressFamily != AddressFamily.InterNetwork)
                    throw new ArgumentException("The specified host address must be IPv4.", nameof(host));

                addr = ip.GetAddressBytes();
            }

            cancellationToken.ThrowIfCancellationRequested();
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            await socket.ConnectAsync(ProxyHost, ProxyPort, cancellationToken);

            try
            {
                var buffer = GetConnectCommand(domain, addr, port);
                await socket.SendAsync(buffer.AsMemory(), SocketFlags.None, cancellationToken);
                // await SendAsync(socket, buffer, 0, buffer.Length, cancellationToken);

                // +-----+-----+----------+----------+
                // | VER | REP | BND.PORT | BND.ADDR |
                // +-----+-----+----------+----------+
                // |  1  |  1  |    2     |    4     |
                // +-----+-----+----------+----------+
                int nread, n = 0;

                do
                {
                    nread = await socket.ReceiveAsync(buffer.AsMemory(0 + n, 8 - n), SocketFlags.None, cancellationToken);
                    if (nread <= 0)
                        throw new EndOfStreamException();


                    n += nread;
                } while (n < 8);

                if (buffer[1] != (byte)Socks4Reply.RequestGranted)
                    throw new ProxyProtocolException(string.Format(CultureInfo.InvariantCulture, "Failed to connect to {0}:{1}: {2}", host, port, GetFailureReason(buffer[1])));

                // TODO: do we care about BND.ADDR and BND.PORT?

                return new NetworkStream(socket, true);
            }
            catch
            {
                if (socket.Connected)
                    await socket.DisconnectAsync(false);

                socket.Dispose();
                throw;
            }
        }
    }
}
