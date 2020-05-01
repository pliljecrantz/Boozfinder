using Boozfinder.Helpers;
using Newtonsoft.Json;

namespace Boozfinder.Models.Data
{
    public class Review
    {
        [JsonProperty(PropertyName = "rating")]
        public Enums.Rating Rating { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "reviewer")]
        public string Reviewer { get; set; }
    }
}
