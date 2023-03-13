using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace QuickProxy
{
   
    public enum ProxyType
    {
        None,
        Http,
        Socks4,
        Socks4a,
        Socks5
    }
    public class ProxyClientFactory
    {

        public IProxyClient CreateProxyClient(ProxyType type)
        {
            if (type == ProxyType.None)
                throw new ArgumentOutOfRangeException("type");

            switch (type)
            {
                case ProxyType.Http:
                    return new HttpProxyClient();
                case ProxyType.Socks4:
                    return new Socks4ProxyClient();
                case ProxyType.Socks4a:
                    return new Socks4aProxyClient();
                case ProxyType.Socks5:
                    return new Socks5ProxyClient();
                default:
                    throw new ProxyException(String.Format("Unknown proxy type {0}.", type.ToString()));
            }
        }        

        public IProxyClient CreateProxyClient(ProxyType type, TcpClient tcpClient)
        {
            if (type == ProxyType.None)
                throw new ArgumentOutOfRangeException("type");
            
            switch (type)
            {
                case ProxyType.Http:
                    return new HttpProxyClient(tcpClient);
                case ProxyType.Socks4:
                    return new Socks4ProxyClient(tcpClient);
                case ProxyType.Socks4a:
                    return new Socks4aProxyClient(tcpClient);
                case ProxyType.Socks5:
                    return new Socks5ProxyClient(tcpClient);
                default:
                    throw new ProxyException(String.Format("Unknown proxy type {0}.", type.ToString()));
            }
        }        
        public IProxyClient CreateProxyClient(ProxyType type, string proxyHost, ushort proxyPort)
        {
            if (type == ProxyType.None)
                throw new ArgumentOutOfRangeException("type");
            
            switch (type)
            {
                case ProxyType.Http:
                    return new HttpProxyClient(proxyHost, proxyPort);
                case ProxyType.Socks4:
                    return new Socks4ProxyClient(proxyHost, proxyPort);
                case ProxyType.Socks4a:
                    return new Socks4aProxyClient(proxyHost, proxyPort);
                case ProxyType.Socks5:
                    return new Socks5ProxyClient(proxyHost, proxyPort);
                default:
                    throw new ProxyException(String.Format("Unknown proxy type {0}.", type.ToString()));
            }
        }

        public IProxyClient CreateProxyClient(ProxyType type, string proxyHost, ushort proxyPort, string proxyUsername, string proxyPassword)
        {
            if (type == ProxyType.None)
                throw new ArgumentOutOfRangeException("type");

            switch (type)
            {
                case ProxyType.Http:
                    return new HttpProxyClient(proxyHost, proxyPort);
                case ProxyType.Socks4:
                    return new Socks4ProxyClient(proxyHost, proxyPort, proxyUsername);
                case ProxyType.Socks4a:
                    return new Socks4aProxyClient(proxyHost, proxyPort, proxyUsername);
                case ProxyType.Socks5:
                    return new Socks5ProxyClient(proxyHost, proxyPort, proxyUsername, proxyPassword);
                default:
                    throw new ProxyException(String.Format("Unknown proxy type {0}.", type.ToString()));
            }
        }

 
        public IProxyClient CreateProxyClient(ProxyType type, TcpClient tcpClient, string proxyHost, ushort proxyPort, string proxyUsername, string proxyPassword)
        {
            IProxyClient c = CreateProxyClient(type, proxyHost, proxyPort, proxyUsername, proxyPassword);
            c.TcpClient = tcpClient;
            return c;
        }


    }



}
