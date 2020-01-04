using System.Collections.Generic;

namespace Boozfinder.Models.Data
{
    public class Booze
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ArticleId { get; set; }
        public IList<Review> Reviews { get; set; }
        public string Type { get; set; }
        public string ImageData { get; set; }
        public bool HasImage { get; set; }
    }
}
