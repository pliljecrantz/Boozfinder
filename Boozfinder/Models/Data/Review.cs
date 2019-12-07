using Boozfinder.Helpers;

namespace Boozfinder.Models.Data
{
    public class Review
    {
        public Enums.Rating Rating { get; set; }
        public string Text { get; set; }
        public string Email { get; set; }
    }
}
