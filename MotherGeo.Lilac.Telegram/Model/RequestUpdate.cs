using Newtonsoft.Json;

namespace MotherGeo.Lilac.Telegram.Model
{
    public class RequestUpdate
    {
        [JsonProperty("update_id")]
        public int UpdateId { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }
    }
}