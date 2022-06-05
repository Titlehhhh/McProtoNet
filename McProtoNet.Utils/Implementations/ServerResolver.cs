using Heijden.Dns.Portable;

namespace McProtoNet.Utils
{
    public class ServerResolver : IServerResolver
    {
        public async Task<(string, ushort)> ResolveAsync(string host)
        {
            ushort port = 0;
            var resolver = new Resolver();
            
            Heijden.DNS.Response response = await (new Resolver()).Query("_minecraft._tcp." + host, Heijden.DNS.QType.SRV);
            Heijden.DNS.RecordSRV[] srvRecords = response.RecordsSRV;
            if (srvRecords != null && srvRecords.Any())
            {
                //Order SRV records by priority and weight, then randomly
                Heijden.DNS.RecordSRV result = srvRecords
                    .OrderBy(record => record.PRIORITY)
                    .ThenByDescending(record => record.WEIGHT)
                    .ThenBy(record => Guid.NewGuid())
                    .First();
                string target = result.TARGET.Trim('.');
                host = target;
                port = result.PORT;
            }
            return (host, port);
        }
    }
}