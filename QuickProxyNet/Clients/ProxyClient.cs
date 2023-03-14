using System.Net;
using System.Net.Sockets;

namespace QuickProxyNet
{
    public abstract class ProxyClient : IProxyClient
    {
        protected ProxyClient(string host, int port)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            if (host.Length == 0 || host.Length > 255)
                throw new ArgumentException("The length of the host name must be between 0 and 256 characters.", nameof(host));

            if (port < 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port));

            ProxyHost = host;
            ProxyPort = port == 0 ? 1080 : port;
        }

        protected ProxyClient(string host, int port, NetworkCredential credentials) : this(host, port)
        {
            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials));

            ProxyCredentials = credentials;
        }

        public NetworkCredential ProxyCredentials
        {
            get; private set;
        }

        public string ProxyHost
        {
            get; private set;
        }

        public int ProxyPort
        {
            get; private set;
        }

        public IPEndPoint LocalEndPoint
        {
            get; set;
        }

        internal static void ValidateArguments(string host, int port)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            if (host.Length == 0 || host.Length > 255)
                throw new ArgumentException("The length of the host name must be between 0 and 256 characters.", nameof(host));

            if (port <= 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port));
        }

        static void ValidateArguments(string host, int port, int timeout)
        {
            ValidateArguments(host, port);

            if (timeout < -1)
                throw new ArgumentOutOfRangeException(nameof(timeout));
        }

        static void AsyncOperationCompleted(object sender, SocketAsyncEventArgs args)
        {
            var tcs = (TaskCompletionSource<bool>)args.UserToken;

            if (args.SocketError == SocketError.Success)
            {
                tcs.TrySetResult(true);
                return;
            }

            tcs.TrySetException(new SocketException((int)args.SocketError));
        }

        internal static void Send(Socket socket, byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            if (cancellationToken.CanBeCanceled)
            {
                var tcs = new TaskCompletionSource<bool>();

                using (var registration = cancellationToken.Register(() => tcs.TrySetCanceled(), false))
                {
                    using (var args = new SocketAsyncEventArgs())
                    {
                        args.Completed += AsyncOperationCompleted;
                        args.SetBuffer(buffer, offset, length);
                        args.AcceptSocket = socket;
                        args.UserToken = tcs;

                        if (!socket.SendAsync(args))
                            AsyncOperationCompleted(null, args);

                        tcs.Task.GetAwaiter().GetResult();
                        return;
                    }
                }
            }

            SocketUtils.Poll(socket, SelectMode.SelectWrite, cancellationToken);

            socket.Send(buffer, offset, length, SocketFlags.None);
        }

        internal static async Task SendAsync(Socket socket, byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            using (var registration = cancellationToken.Register(() => tcs.TrySetCanceled(), false))
            {
                using (var args = new SocketAsyncEventArgs())
                {
                    args.Completed += AsyncOperationCompleted;
                    args.SetBuffer(buffer, offset, length);
                    args.AcceptSocket = socket;
                    args.UserToken = tcs;

                    if (!socket.SendAsync(args))
                        AsyncOperationCompleted(null, args);

                    await tcs.Task.ConfigureAwait(false);
                }
            }
        }

        internal static int Receive(Socket socket, byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            if (cancellationToken.CanBeCanceled)
            {
                var tcs = new TaskCompletionSource<bool>();

                using (var registration = cancellationToken.Register(() => tcs.TrySetCanceled(), false))
                {
                    using (var args = new SocketAsyncEventArgs())
                    {
                        args.Completed += AsyncOperationCompleted;
                        args.SetBuffer(buffer, offset, length);
                        args.AcceptSocket = socket;
                        args.UserToken = tcs;

                        if (!socket.ReceiveAsync(args))
                            AsyncOperationCompleted(null, args);

                        tcs.Task.GetAwaiter().GetResult();

                        return args.BytesTransferred;
                    }
                }
            }

            SocketUtils.Poll(socket, SelectMode.SelectRead, cancellationToken);

            return socket.Receive(buffer, offset, length, SocketFlags.None);
        }

        internal static async Task<int> ReceiveAsync(Socket socket, byte[] buffer, int offset, int length, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            using (var registration = cancellationToken.Register(() => tcs.TrySetCanceled(), false))
            {
                using (var args = new SocketAsyncEventArgs())
                {
                    args.Completed += AsyncOperationCompleted;
                    args.SetBuffer(buffer, offset, length);
                    args.AcceptSocket = socket;
                    args.UserToken = tcs;

                    if (!socket.ReceiveAsync(args))
                        AsyncOperationCompleted(null, args);

                    await tcs.Task.ConfigureAwait(false);

                    return args.BytesTransferred;
                }
            }
        }

        public abstract Stream Connect(string host, int port, CancellationToken cancellationToken = default(CancellationToken));

        public abstract Task<Stream> ConnectAsync(string host, int port, CancellationToken cancellationToken = default(CancellationToken));

        public virtual Stream Connect(string host, int port, int timeout, CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateArguments(host, port, timeout);

            using (var ts = new CancellationTokenSource(timeout))
            {
                using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ts.Token))
                {
                    try
                    {
                        return Connect(host, port, linked.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                            throw new TimeoutException();
                        throw;
                    }
                }
            }
        }

        public async virtual Task<Stream> ConnectAsync(string host, int port, int timeout, CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateArguments(host, port, timeout);

            using (var ts = new CancellationTokenSource(timeout))
            {
                using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ts.Token))
                {
                    try
                    {
                        return await ConnectAsync(host, port, linked.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                            throw new TimeoutException();
                        throw;
                    }
                }
            }
        }
    }
}
