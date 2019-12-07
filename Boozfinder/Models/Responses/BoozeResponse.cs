using Boozfinder.Models.Data;

namespace Boozfinder.Models.Responses
{
    public class BoozeResponse
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public Booze Item { get; set; }        
    }
}
