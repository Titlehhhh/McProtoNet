using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;

namespace ProtoLib.API.Types.Chat
{
    [DataContract]
    public class ChatMessage
    {
        private static readonly DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ChatMessage));
        private List<ChatMessage> extra = new List<ChatMessage>();

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "color")]
        public string Color { get; set; }

        [DataMember(Name = "bold")]
        public bool Bold { get; set; }

        [DataMember(Name = "italic")]
        public bool Italic { get; set; }

        [DataMember(Name = "underlined")]
        public bool Underlined { get; set; }

        [DataMember(Name = "strikethrough")]
        public bool Strikethrough { get; set; }

        [DataMember(Name = "obfuscated")]
        public bool Obfuscated { get; set; }

        [DataMember(Name = "insertion")]
        public string Insertion { get; set; }

        //TODO
        //[IgnoreDataMember]
        [DataMember(Name = "clickEvent")]
        public ClickComponent ClickEvent { get; set; }
        //TODO
        // [IgnoreDataMember]
        [DataMember(Name = "hoverEvent")]
        public HoverComponent HoverEvent { get; set; }

        [DataMember(Name = "extra", EmitDefaultValue = true)]
        public List<ChatMessage> Extra
        {
            get
            {
                if (extra == null)
                    extra = new List<ChatMessage>();
                return extra;
            }
            set
            {
                if (value != null)
                    extra = value;
            }
        }
        public IEnumerable<ChatMessage> Extras => GetExtras();

        public IEnumerable<ChatMessage> GetExtras()
        {
            if (Extra == null)
                yield break;

            foreach (var extra in Extra)
            {
                yield return extra;
            }
        }

        public static implicit operator ChatMessage(string text) => Simple(text);

        public static ChatMessage operator +(ChatMessage a, ChatMessage b) => a.AddExtra(b);

        public static ChatMessage operator +(ChatMessage a, ChatColor b) => a.AppendColor(b);

        public static ChatMessage Simple(string text) => new() { Text = text };

        public static ChatMessage SimpleLegacy(string text) => new() { Text = ReformatAmpersandPrefixes(text) };

        public static ChatMessage Simple(string text, ChatColor color) => new()
        {
            Text = $"{color}{text}"
        };

        public static ChatMessage SimpleLegacy(string text, ChatColor color) => new()
        {
            Text = $"{color}{ReformatAmpersandPrefixes(text)}"
        };

        public static ChatMessage Click(ChatMessage message, EClickAction action, string value, string translate = "")
        {
            message.ClickEvent = new ClickComponent(action, value, translate);
            return message;
        }

        public static ChatMessage Hover(ChatMessage message, EHoverAction action, object contents, string translate = "")
        {
            message.HoverEvent = new HoverComponent(action, contents, translate);
            return message;
        }

        public static string ReformatAmpersandPrefixes(string originalText)
        {
            return string.Create(originalText.Length, originalText, (span, text) =>
            {
                for (int i = 0; i < span.Length; i++)
                {
                    char c = text[i];
                    span[i] = c;

                    if (c == '&' && i + 1 < text.Length)
                    {
                        c = text[i + 1];
                        if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'e') || (c >= 'k' && c <= 'o') || c == 'r')
                        {
                            span[i] = '§';
                        }
                    }
                }
            });
        }

        public ChatMessage AddExtra(ChatMessage message)
        {
            Extra ??= new List<ChatMessage>();
            Extra.Add(message);

            return this;
        }

        public ChatMessage AddExtra(List<ChatMessage> messages)
        {
            Extra ??= new List<ChatMessage>(capacity: messages.Count);
            Extra.AddRange(messages);

            return this;
        }

        public ChatMessage AddExtra(IEnumerable<ChatMessage> messages)
        {
            foreach (var message in messages)
            {
                AddExtra(message);
            }

            return this;
        }

        public ChatMessage AppendText(string text)
        {
            if (Text is null)
            {
                Text = text;
            }
            else
            {
                Text += text;
            }
            return this;
        }

        public ChatMessage AppendColor(ChatColor color)
        {
            if (Text is null)
            {
                Text = color.ToString();
            }
            else
            {
                Text += color.ToString();
            }

            return this;
        }

        public ChatMessage AppendText(string text, ChatColor color)
        {
            if (Text is null)
            {
                Text = $"{color}{text}";
            }
            else
            {
                Text += $"{color}{text}";
            }
            return this;
        }
        public ChatMessage()
        {
            // Extra = new List<ChatMessage>();
        }

        public static ChatMessage Parse(string json)
        {
            return (ChatMessage)serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(json)));
        }
        public override string ToString()
        {
            return string.Join("", Extra.Select(x => x.Text));
        }


        public static ChatMessage Empty => Simple(string.Empty);

        public string ToString(JsonSerializerOptions options) => JsonSerializer.Serialize(this, options);
    }
}
