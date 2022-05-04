using System.Runtime.Serialization;

namespace McProtoNet.API.Types.Chat
{
    [DataContract]
    public class ClickComponent
    {
        [DataMember(Name = "action")]
        public EClickAction Action { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }

        public string Translate { get; set; }

        public ClickComponent(EClickAction action, string value, string translate = "")
        {
            Action = action;
            Value = value;
            Translate = translate;
        }
    }
}
