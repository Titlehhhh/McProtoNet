namespace McProtoNet.Services
{
    public class ServerResolver : IServerResolver
    {
        public async Task<(string, ushort)> ResolveAsync(string host)
        {
            ushort port = 0;
            await Task.Run(() =>
            {
                Heijden.DNS.Response response = new Heijden.DNS.Resolver().Query("_minecraft._tcp." + host, Heijden.DNS.QType.SRV);
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

            });
            return (host, port);
        }
    }
}