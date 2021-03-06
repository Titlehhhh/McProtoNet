using System.Runtime.Serialization;

namespace McProtoNet.Utils
{
    [DataContract]
    public class ServerInfo
    {
        [DataMember(Name = "players", EmitDefaultValue = true)]
        public PlayerInfo Players { get; set; }

        [DataMember(Name = "version")]
        public VersionInfo TargetVersion { get; set; }

        [DataMember(Name = "description", EmitDefaultValue = true)]
        public string Description { get; set; }

        [DataMember(Name = "favicon")]
        public string Icon { get; set; }

        public override string ToString()
        {
            string header = $"Сервер версия {TargetVersion.StringVersion} {Players.OnlinePlayers}/{Players.MaxPlayers}";
            string desc = "Описание: \n" + Description.ToString();
            string players = "Игроки:\n" + string.Join("\n", Players.PlayerList.Select(x => x.Nickname));
            return header + "\n" + desc + "\n" + players;
        }
    }
}