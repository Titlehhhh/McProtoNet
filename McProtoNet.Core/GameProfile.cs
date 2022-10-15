namespace McProtoNet.Core
{


    public class GameProfile
    {
        public Guid UUID { get; private set; }
        public string? Username { get; set; }

        public GameProfile(Guid uUID, string? username)
        {
            UUID = uUID;
            Username = username;
        }
    }
}
