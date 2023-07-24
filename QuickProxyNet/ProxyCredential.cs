using System.Runtime.Serialization;

namespace QuickProxyNet
{
	[DataContract]
	public sealed class ProxyCredential
	{
		[DataMember]
		public string Login { get; set; }
		[DataMember]
		public string Password { get; set; }
	}
}
