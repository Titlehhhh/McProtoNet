using System.Net;
using System.Runtime.CompilerServices;

namespace QuickProxyNet
{
	public static class Ext
    {

        public static NetworkCredential CreateFrom(this ProxyCredential proxyCredential)
        {
            return new NetworkCredential(proxyCredential.Login, proxyCredential.Password);
        }

        private static ProxyClientFactory clientFactory = new();
        public static IProxyClient CreateClient(this ProxyInfo proxy)
        {
            if (proxy.Credential is null)
                return clientFactory.Create(proxy.Type, proxy.Host, proxy.Port);
            return clientFactory.Create(proxy.Type, proxy.Host, proxy.Port, proxy.Credential.CreateFrom());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static async ValueTask<int> ReadToEndAsync(this Stream stream, Memory<byte> buffer, int length, CancellationToken token)
        {
            int totalRead = 0;
            while (totalRead < length)
            {
                int read = await stream.ReadAsync(buffer.Slice(totalRead), token);
                if (read <= 0)
                    throw new EndOfStreamException();

                totalRead += read;
            }

            return totalRead;
        }


    }
}
