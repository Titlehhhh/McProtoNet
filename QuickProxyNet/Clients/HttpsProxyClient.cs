using System.Buffers;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace QuickProxyNet
{

    public class HttpsProxyClient : ProxyClient
    {
        const SslProtocols DefaultSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;

        const int BufferSize = 4096;

        public HttpsProxyClient(string host, int port) : base(host, port)
        {
            SslProtocols = DefaultSslProtocols;
        }

        public HttpsProxyClient(string host, int port, NetworkCredential credentials) : base(host, port, credentials)
        {
            SslProtocols = DefaultSslProtocols;
        }

        public SslProtocols SslProtocols
        {
            get; set;
        }

        public CipherSuitesPolicy SslCipherSuitesPolicy
        {
            get; set;
        }

        public X509CertificateCollection ClientCertificates
        {
            get; set;
        }

        public bool CheckCertificateRevocation
        {
            get; set;
        }

        public RemoteCertificateValidationCallback ServerCertificateValidationCallback
        {
            get; set;
        }

        // Note: This is used by SslHandshakeException to build the exception message.
        SslCertificateValidationInfo sslValidationInfo;

        bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool valid;

            sslValidationInfo?.Dispose();
            sslValidationInfo = null;

            if (ServerCertificateValidationCallback != null)
            {
                valid = ServerCertificateValidationCallback(ProxyHost, certificate, chain, sslPolicyErrors);

            }
            else if (ServicePointManager.ServerCertificateValidationCallback != null)
            {
                valid = ServicePointManager.ServerCertificateValidationCallback(ProxyHost, certificate, chain, sslPolicyErrors);

            }
            else
            {
                valid = sslPolicyErrors == SslPolicyErrors.None;
            }

            if (!valid)
            {
                // Note: The SslHandshakeException.Create() method will nullify this once it's done using it.
                sslValidationInfo = new SslCertificateValidationInfo(sender, certificate, chain, sslPolicyErrors);
            }

            return valid;
        }

        SslClientAuthenticationOptions GetSslClientAuthenticationOptions(string host, RemoteCertificateValidationCallback remoteCertificateValidationCallback)
        {
            return new SslClientAuthenticationOptions
            {
                CertificateRevocationCheckMode = CheckCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck,
                ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http11 },
                RemoteCertificateValidationCallback = remoteCertificateValidationCallback,

                CipherSuitesPolicy = SslCipherSuitesPolicy,

                ClientCertificates = ClientCertificates,
                EnabledSslProtocols = SslProtocols,
                TargetHost = host
            };
        }

        public override Stream Connect(string host, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateArguments(host, port);

            cancellationToken.ThrowIfCancellationRequested();
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(ProxyHost, ProxyPort);
            var ssl = new SslStream(new NetworkStream(socket, true), false, ValidateRemoteCertificate);

            try
            {
                ssl.AuthenticateAsClient(GetSslClientAuthenticationOptions(host, ValidateRemoteCertificate));

            }
            catch (Exception ex)
            {
                ssl.Dispose();

                throw SslHandshakeException.Create(ref sslValidationInfo, ex, false, "HTTP", host, port, 443, 80);
            }

            var command = HttpProxyClient.GetConnectCommand(host, port, ProxyCredentials);

            try
            {
                ssl.Write(command, 0, command.Length);

                var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
                var builder = new StringBuilder();

                try
                {
                    var newline = false;

                    // read until we consume the end of the headers (it's ok if we read some of the content)
                    do
                    {
                        int nread = ssl.Read(buffer, 0, BufferSize);
                        int index = 0;

                        if (HttpProxyClient.TryConsumeHeaders(builder, buffer, ref index, nread, ref newline))
                            break;
                    } while (true);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }

                HttpProxyClient.ValidateHttpResponse(builder, host, port);
                return ssl;
            }
            catch
            {
                ssl.Dispose();
                throw;
            }
        }

        public override async Task<Stream> ConnectAsync(string host, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateArguments(host, port);

            cancellationToken.ThrowIfCancellationRequested();
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            await socket.ConnectAsync(ProxyHost, ProxyPort, cancellationToken);
            var ssl = new SslStream(new NetworkStream(socket, true), false, ValidateRemoteCertificate);

            try
            {
                await ssl.AuthenticateAsClientAsync(GetSslClientAuthenticationOptions(host, ValidateRemoteCertificate), cancellationToken);

            }
            catch (Exception ex)
            {
                ssl.Dispose();

                throw SslHandshakeException.Create(ref sslValidationInfo, ex, false, "HTTP", host, port, 443, 80);
            }

            var command = HttpProxyClient.GetConnectCommand(host, port, ProxyCredentials);

            try
            {
                await ssl.WriteAsync(command, 0, command.Length, cancellationToken);

                var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
                var builder = new StringBuilder();

                try
                {
                    var newline = false;

                    // read until we consume the end of the headers (it's ok if we read some of the content)
                    do
                    {
                        int nread = ssl.Read(buffer, 0, BufferSize);
                        int index = 0;

                        if (HttpProxyClient.TryConsumeHeaders(builder, buffer, ref index, nread, ref newline))
                            break;
                    } while (true);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }

                HttpProxyClient.ValidateHttpResponse(builder, host, port);
                return ssl;
            }
            catch
            {
                ssl.Dispose();
                throw;
            }
        }
    }
}
