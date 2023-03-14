using System.Net;
using System.Net.Sockets;

namespace QuickProxyNet
{
    static class SocketUtils
    {
        public static Socket Connect(string host, int port, IPEndPoint localEndPoint, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var ipAddresses = Dns.GetHostAddresses(host);

            for (int i = 0; i < ipAddresses.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var socket = new Socket(ipAddresses[i].AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    if (localEndPoint != null)
                        socket.Bind(localEndPoint);

                    if (cancellationToken.CanBeCanceled)
                    {
                        var tcs = new TaskCompletionSource<bool>();

                        using (var registration = cancellationToken.Register(() => tcs.TrySetCanceled(), false))
                        {
                            var ar = socket.BeginConnect(ipAddresses[i], port, e => tcs.TrySetResult(true), null);
                            tcs.Task.GetAwaiter().GetResult();
                            socket.EndConnect(ar);
                        }
                    }
                    else
                    {
                        socket.Connect(ipAddresses[i], port);
                    }

                    return socket;
                }
                catch (OperationCanceledException)
                {
                    socket.Dispose();
                    throw;
                }
                catch
                {
                    socket.Dispose();

                    if (i + 1 == ipAddresses.Length)
                        throw;
                }
            }

            throw new IOException(string.Format("Failed to resolve host: {0}", host));
        }

        public static async Task<Socket> ConnectAsync(string host, int port, IPEndPoint localEndPoint, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var ipAddresses = await Dns.GetHostAddressesAsync(host).ConfigureAwait(false);

            for (int i = 0; i < ipAddresses.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var socket = new Socket(ipAddresses[i].AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    if (localEndPoint != null)
                        socket.Bind(localEndPoint);

                    var tcs = new TaskCompletionSource<bool>();

                    using (var registration = cancellationToken.Register(() => tcs.TrySetCanceled(), false))
                    {
                        var ar = socket.BeginConnect(ipAddresses[i], port, e => tcs.TrySetResult(true), null);
                        await tcs.Task.ConfigureAwait(false);
                        socket.EndConnect(ar);
                    }

                    return socket;
                }
                catch (OperationCanceledException)
                {
                    socket.Dispose();
                    throw;
                }
                catch
                {
                    socket.Dispose();

                    if (i + 1 == ipAddresses.Length)
                        throw;
                }
            }

            throw new IOException(string.Format("Failed to resolve host: {0}", host));
        }

        public static Socket Connect(string host, int port, IPEndPoint localEndPoint, int timeout, CancellationToken cancellationToken)
        {
            using (var ts = new CancellationTokenSource(timeout))
            {
                using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ts.Token))
                {
                    try
                    {
                        return Connect(host, port, localEndPoint, linked.Token);
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

        public static async Task<Socket> ConnectAsync(string host, int port, IPEndPoint localEndPoint, int timeout, CancellationToken cancellationToken)
        {
            using (var ts = new CancellationTokenSource(timeout))
            {
                using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ts.Token))
                {
                    try
                    {
                        return await ConnectAsync(host, port, localEndPoint, linked.Token).ConfigureAwait(false);
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

        public static void Poll(Socket socket, SelectMode mode, CancellationToken cancellationToken)
        {
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                // wait 1/4 second and then re-check for cancellation
            } while (!socket.Poll(250000, mode));
        }
    }
}
