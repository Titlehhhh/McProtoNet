using System.Runtime.Serialization;

namespace ProtoLib.API
{

    [DataContract]
    public class GameProfile
    {
        [DataMember(Name = "id")]
        internal string uuid;
        [DataMember(Name = "name")]
        internal string nick;


        public string UUID { get; private set; }


        public string Nickname { get; set; }


        public GameProfile(string uUID, string nickname)
        {
            UUID = uUID;
            Nickname = nickname;
        }
    }
}
