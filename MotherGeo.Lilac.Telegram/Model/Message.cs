using Newtonsoft.Json;

namespace MotherGeo.Lilac.Telegram.Model
{
    public class Message
    {
        [JsonProperty("update_id")]
        public long UpdateId { get; set; }
        
        [JsonProperty("chat")]
        public Chat Chat { get; set; }
        
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}