using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace McProtoNet.Utils
{
    [DataContract]
    public class ServerInfo
    {
        [DataMember(Name = "version", Order =0)]
        public VersionInfo TargetVersion { get; set; }

        [DataMember(Name = "players", EmitDefaultValue = true, Order = 1)]
        public PlayerInfo Players { get; set; }

        [DataMember(Name = "description", EmitDefaultValue = true, Order = 2)]
        public ChatMessage Description { get; set; }




        [DataMember(Name = "favicon", Order = 3)]
        public string Icon { get; set; }

        private static DataContractJsonSerializer serializer = new(typeof(ServerInfo));
        public override string ToString()
        {
            using(MemoryStream ms = new())
            {
                serializer.WriteObject(ms, this);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            string header = $"Сервер версия {TargetVersion.StringVersion} {Players.OnlinePlayers}/{Players.MaxPlayers}";
            string desc = "Описание: \n" + Description.ToString();
            string players = "Игроки:\n" + string.Join("\n", Players.PlayerList.Select(x => x.Username));
            return header + "\n" + desc + "\n" + players;
        }
    }
}