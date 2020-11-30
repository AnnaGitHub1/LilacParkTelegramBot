using Newtonsoft.Json;

namespace MotherGeo.Lilac.Telegram.Model
{
    public class Chat
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
    }
}