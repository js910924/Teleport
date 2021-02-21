using Newtonsoft.Json;

namespace Teleport.Entities
{
    public class TelegramRequest
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("chat_id")]
        public string ChatId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}