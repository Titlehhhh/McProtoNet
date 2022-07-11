
using Starksoft.Aspen.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.Utils
{

    public abstract class NetService
    {
        protected ITcpClientFactory TcpTactory { get; private set; } = new TcpClientFactory();


        public void SetProxy(ProxyType type, string host, ushort port)
        {
            this.TcpTactory = new ProxyTcpClientFactory(host, port, type);
        }
    }
}
