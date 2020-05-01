using Newtonsoft.Json;
using System.Collections.Generic;

namespace Boozfinder.Models.Data
{
    public class Booze
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "articleId")]
        public string ArticleId { get; set; }

        [JsonProperty(PropertyName = "reviews")]
        public IList<Review> Reviews { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "imageData")]
        public string ImageData { get; set; }

        [JsonProperty(PropertyName = "hasImage")]
        public bool HasImage { get; set; }

        [JsonProperty(PropertyName = "creator")]
        public bool Creator { get; set; }
    }
}
