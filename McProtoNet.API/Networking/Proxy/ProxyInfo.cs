using System.Net;
using System.Runtime.Serialization;

namespace McProtoNet.API.Networking.Proxy
{
    [DataContract]
    public struct ProxyInfo
    {

        public IPEndPoint EndPoint { get; private set; }
        public ProxyType ProxyType { get; private set; }
        public bool Authenticated { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public ProxyInfo(IPEndPoint endPoint, ProxyType proxyType, bool authenticated, string username, string password)
        {
            EndPoint = endPoint;
            ProxyType = proxyType;
            Authenticated = authenticated;
            Username = username;
            Password = password;
        }
    }
    public enum ProxyType
    {
        HTTP,
        SOCKS4,
        SOCKS5
    }
}
