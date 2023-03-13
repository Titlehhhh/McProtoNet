using System;
using System.Text;
using System.Globalization;
using System.Net.Sockets;

namespace QuickProxy
{
    internal static class Utils
    {
        internal static int ReadToEnd(this Stream stream, Span<byte> buffer, int length)
        {
            int totalRead = 0;
            while (totalRead < length)
            {
                int read = stream.Read(buffer.Slice(totalRead));


                totalRead += read;
            }

            return totalRead;
        }
        internal static async ValueTask<int> ReadToEndAsync(this Stream stream, Memory<byte> buffer, int length, CancellationToken token)
        {
            int totalRead = 0;
            while (totalRead < length)
            {
                int read = await stream.ReadAsync(buffer.Slice(totalRead), token);


                totalRead += read;
            }

            return totalRead;
        }
        internal static string GetHost(TcpClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            string host = "";
            try
            {
                host = ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            }
            catch
            { };

            return host;
        }

        internal static string GetPort(TcpClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            string port = "";
            try
            {
                port = ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Port.ToString(CultureInfo.InvariantCulture);
            }
            catch
            { };

            return port;
        }

    }
}
