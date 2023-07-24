using System.Net;
using System.Runtime.Serialization;

namespace QuickProxyNet
{
	[DataContract]
	public sealed class ProxyInfo
    {
		[DataMember]
        public string Host { get; set; }
		[DataMember]
		public ushort Port { get; set; }
		[DataMember]
		public ProxyType Type { get; set; }

		[DataMember]
		public ProxyCredential? Credential { get; set; } = new();

		
	}
}
