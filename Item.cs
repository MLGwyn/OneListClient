using System;
using System.Text.Json.Serialization;

namespace OneListClient
{
    public class Item
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("complete")]
        public bool Complete { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime Created_at { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime Updated_at { get; set; }

        public string CompletedStatus
        {
            get
            {
                // Uses a ternary to return "completed" if the `complete` variable is true, returns "not completed" if false
                return Complete ? "completed" : "not completed";
            }
        }
    }
}
